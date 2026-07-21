using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;
using IvanWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web; // Necessário para formatar a URL do WhatsApp

namespace IvanWeb.Api.Controllers;

public class BuyItemRequest
{
    public Guid PlayerId { get; set; }
    public Guid StoreItemId { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class StoreController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITrophyEngineService _trophyEngine;

    public StoreController(ApplicationDbContext dbContext, ITrophyEngineService trophyEngine)
    {
        _dbContext = dbContext;
        _trophyEngine = trophyEngine;
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetAvailableItems([FromQuery] Guid playerId)
    {
        var player = await _dbContext.PlayerProfiles
            .Include(p => p.Purchases)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return NotFound("Perfil não encontrado.");

        // Pega os IDs de todos os itens que ela já comprou
        var purchasedItemIds = player.Purchases.Select(p => p.StoreItemId).ToHashSet();

        // Busca o catálogo ativo da loja
        var allItems = await _dbContext.StoreItems
            .Where(i => i.IsActive)
            .ToListAsync();

        // O Filtro de Ouro:
        // Se o item for "IsSinglePurchase" E já estiver na lista de compras dela, 
        // ele é removido da vitrine (ex: "Desbloquear Sala de Troféus").
        var availableItems = allItems.Where(item =>
            !(item.IsSinglePurchase && purchasedItemIds.Contains(item.Id))
        ).ToList();

        // Mapeia para um objeto anônimo enxuto para o Flutter
        return Ok(new
        {
            CurrentScore = player.TotalScore,
            Items = availableItems.Select(i => new
            {
                i.Id,
                i.Name,
                i.Price,
                i.Icon,
                i.IsSinglePurchase,
                i.SendWhatsApp
            })
        });
    }

    [HttpPost("buy")]
    public async Task<IActionResult> BuyItem([FromBody] BuyItemRequest request)
    {
        var player = await _dbContext.PlayerProfiles
            .Include(p => p.Purchases)
            .FirstOrDefaultAsync(p => p.Id == request.PlayerId);

        if (player == null) return NotFound("Perfil não encontrado.");

        var item = await _dbContext.StoreItems.FindAsync(request.StoreItemId);
        if (item == null || !item.IsActive) return NotFound("Item indisponível.");

        // 1. REGRA 1: Validação de Saldo
        if (player.TotalScore < item.Price)
        {
            return BadRequest(new { Success = false, Message = "Pontos insuficientes. Continue jogando para ganhar mais!" });
        }

        // 2. REGRA 2: Bloqueio de Compra Dupla (para itens especiais)
        if (item.IsSinglePurchase && player.Purchases.Any(p => p.StoreItemId == item.Id))
        {
            return BadRequest(new { Success = false, Message = "Você já possui este item especial!" });
        }

        // 3. Efetua a Compra
        player.TotalScore -= item.Price;

        var purchase = new PurchaseHistory
        {
            PlayerProfileId = player.Id,
            StoreItemId = item.Id
        };

        // Assume-se que o DbContext tem o DbSet<PurchaseHistory> (ou você pode usar _dbContext.Set<PurchaseHistory>())
        _dbContext.Set<PurchaseHistory>().Add(purchase);
        await _dbContext.SaveChangesAsync();

        // 4. REGRA 3: O Link do WhatsApp ("PAGA NOIS")
        string? whatsAppLink = null;
        if (item.SendWhatsApp)
        {
            string text = $"Meu amor eu adquiri: {item.Name}. Você tem 30 dias para fazer a entrega do prêmio. PAGA NOIS 🎉🎉🎉";
            // Usa o UrlEncode para que espaços e emojis funcionem perfeitamente no navegador do celular dela
            whatsAppLink = $"https://wa.me/?text={HttpUtility.UrlEncode(text)}";
        }

        // 1. Calcula o total de compras feitas até agora
        int totalPurchasesCount = player.Purchases.Count;

        // 2. Calcula o total de pontos que ela já gastou na loja ao longo da vida.
        // Usamos uma query leve no banco para somar os preços cruzando com a tabela StoreItems.
        int totalPointsSpent = await _dbContext.Set<PurchaseHistory>()
            .Where(p => p.PlayerProfileId == player.Id)
            .Include(p => p.StoreItem)
            .SumAsync(p => p.StoreItem!.Price);

        // 3. Monta o evento fotográfico e aciona o Motor
        var storeEvent = new StorePurchaseEvent(totalPurchasesCount, totalPointsSpent);
        var unlockedTrophies = await _trophyEngine.ProcessTrophiesAsync(player.Id, storeEvent);

        // 4. Retorna a lista mapeada para o Flutter disparar os confetes
        return Ok(new
        {
            Success = true,
            Message = "Compra efetuada com sucesso!",
            NewTotalScore = player.TotalScore,
            WhatsAppLink = whatsAppLink,
            TrofeusDesbloqueados = unlockedTrophies.Select(t => new { t.Name, t.Description, t.Icon })
        });
    }
}