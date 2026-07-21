using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;
using IvanWeb.Infrastructure.Data; // Para acessar o DbContext
using Microsoft.EntityFrameworkCore;

namespace IvanWeb.Application.Services;

public class TrophyEngineService : ITrophyEngineService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEnumerable<ITrophyRule> _rules;

    // Injetamos o contexto do banco e a lista de TODAS as regras cadastradas
    public TrophyEngineService(ApplicationDbContext dbContext, IEnumerable<ITrophyRule> rules)
    {
        _dbContext = dbContext;
        _rules = rules;
    }

    public async Task<List<Trophy>> ProcessTrophiesAsync(Guid playerId, object eventContext)
    {
        // 1. Busca o jogador e os troféus que ele já possui
        var player = await _dbContext.PlayerProfiles
            .Include(p => p.UnlockedTrophies)
            .ThenInclude(ut => ut.Trophy) // Precisamos saber quais troféus ele tem
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return new List<Trophy>();

        // Slugs dos troféus que a Rafaela já desbloqueou (para não desbloquear de novo)
        var alreadyUnlockedSlugs = player.UnlockedTrophies
            .Where(ut => ut.Trophy != null)
            .Select(ut => ut.Trophy!.Slug)
            .ToHashSet();

        var newUnlockedSlugs = new HashSet<string>();

        // 2. Roda todas as regras do Motor!
        foreach (var rule in _rules)
        {
            var results = rule.Evaluate(player, eventContext);
            foreach (var slug in results)
            {
                // Se a regra disse que ganhou, e ela ainda não tinha esse troféu...
                if (!alreadyUnlockedSlugs.Contains(slug))
                {
                    newUnlockedSlugs.Add(slug);
                }
            }
        }

        // =========================================================
        // NOVA REGRA: EVENTO DE LOGIN (Primeiro Passo / Boas Vindas)
        // =========================================================
        if (eventContext is AppLoginEvent loginEvent && loginEvent.IsFirstLogin)
        {
            // Busca o troféu "Primeiro Passo" no banco para pegarmos o Slug exato dele
            var primeiroPassoTrophy = await _dbContext.Trophies
                .FirstOrDefaultAsync(t => t.Name == "Primeiro Passo");

            // Se o troféu existir no banco e ela ainda não tiver ganho, adiciona aos novos troféus!
            if (primeiroPassoTrophy != null && !alreadyUnlockedSlugs.Contains(primeiroPassoTrophy.Slug))
            {
                newUnlockedSlugs.Add(primeiroPassoTrophy.Slug);
            }
        }
        // =========================================================

        var newlyUnlockedTrophies = new List<Trophy>();

        // 3. Se houver novos troféus, busca eles no banco e adiciona ao perfil
        if (newUnlockedSlugs.Any())
        {
            var trophiesToUnlock = await _dbContext.Trophies
                .Where(t => newUnlockedSlugs.Contains(t.Slug))
                .ToListAsync();

            foreach (var trophy in trophiesToUnlock)
            {
                var unlockRecord = new UnlockedTrophy
                {
                    PlayerProfileId = playerId,
                    TrophyId = trophy.Id,
                    UnlockedAt = DateTime.UtcNow
                };

                _dbContext.UnlockedTrophies.Add(unlockRecord);
                newlyUnlockedTrophies.Add(trophy);
            }

            // 4. Salva no banco de dados
            await _dbContext.SaveChangesAsync();
        }

        // Retorna a lista de troféus recém-ganhos para a API poder mostrar a notificação (os confetes!) no frontend
        return newlyUnlockedTrophies;
    }
}