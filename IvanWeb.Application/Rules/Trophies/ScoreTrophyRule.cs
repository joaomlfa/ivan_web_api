using IvanWeb.Application.Interfaces;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Rules.Trophies;

public class ScoreTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile profile, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        // Usando o Slug (identificador de texto) em vez do ID numérico
        if (profile.TotalScore >= 100) unlockedSlugs.Add("PRIMEIROS_PONTOS");
        if (profile.TotalScore >= 500) unlockedSlugs.Add("ESCALANDO");
        if (profile.TotalScore >= 1000) unlockedSlugs.Add("MILHAR");
        if (profile.TotalScore >= 5000) unlockedSlugs.Add("CINCO_MIL");
        if (profile.TotalScore >= 10000) unlockedSlugs.Add("DEZ_MIL");
        if (profile.TotalScore >= 25000) unlockedSlugs.Add("VINTE_CINCO_MIL");
        // ... e assim por diante para os outros marcos

        return unlockedSlugs;
    }
}