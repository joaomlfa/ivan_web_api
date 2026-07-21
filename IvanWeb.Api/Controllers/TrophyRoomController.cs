using IvanWeb.Domain.Entities;
using IvanWeb.Infrastructure.Data;
// Adicione o namespace de onde vem a classe EmbeddedTrofeus
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanWeb.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TrophyRoomController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public TrophyRoomController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetTrophyRoom(Guid playerId)
    {
        var player = await _dbContext.PlayerProfiles
         .Include(p => p.UnlockedTrophies)
         .Include(p => p.Purchases)
             .ThenInclude(p => p.StoreItem)
         .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return NotFound("Perfil não encontrado.");

        // Busca o catálogo completo original do seu sistema
        var allTrophies = await _dbContext.Trophies.ToListAsync();

        bool hasBoughtRoom = player.Purchases.Any(p => p.StoreItem?.Name == "Desbloquear Sala de Troféus");

        if (!hasBoughtRoom)
        {
            // Se não comprou, devolvemos apenas o aviso do cadeado!
            return Ok(new { IsLocked = true, Message = "Você precisa comprar a chave na Amor Store primeiro! 💖" });
        }
        // Pega os IDs ou Slugs que a Rafaela já possui
        // (Ajuste "TrophyId" para a propriedade exata que liga a tabela UnlockedTrophy ao catálogo)
        var unlockedIdentifiers = player.UnlockedTrophies.Select(t => t.TrophyId).ToHashSet();

        // Faz o mapeamento e aplica a regra dos "Troféus Secretos"
        var mappedTrophies = allTrophies.Select(t =>
        {
            bool isUnlocked = unlockedIdentifiers.Contains(t.Id);
            var unlockedData = player.UnlockedTrophies.FirstOrDefault(ut => ut.TrophyId == t.Id);

            return new
            {
                t.Id,
                Category = t.Category.ToString(),
                Rarity = t.Rarity.ToString(),
                // Se for secreto e estiver bloqueado, esconde o ícone e o nome
                Icon = isUnlocked || !t.IsSecret ? t.Icon : "❓",
                Name = isUnlocked || !t.IsSecret ? t.Name : "Troféu Secreto",
                // Se estiver bloqueado, mostra o requisito. Se for secreto bloqueado, mostra só a dica.
                Description = isUnlocked ? t.Description : (t.IsSecret ? $"Dica: {t.SecretHint}" : t.Requirements),
                t.Points,
                IsUnlocked = isUnlocked,
                t.IsSecret,
                UnlockedAt = unlockedData?.UnlockedAt
            };
        }).ToList();

        int unlockedCount = mappedTrophies.Count(t => t.IsUnlocked);
        int totalCount = mappedTrophies.Count;
        int percentage = totalCount > 0 ? (unlockedCount * 100) / totalCount : 0;

        // Entregamos o Dashboard completo e mastigado para o Flutter
        return Ok(new
        {
            Progress = new
            {
                Unlocked = unlockedCount,
                Total = totalCount,
                Percentage = percentage
            },
            Trophies = mappedTrophies
        });
    }
}