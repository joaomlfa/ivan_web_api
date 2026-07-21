using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Rules;

public class ClockTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile player, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        if (eventContext is not ClockCompletedEvent clockEvent)
            return unlockedSlugs;

        // O Troféu de Boas-Vindas ao Modo
        // O Motor sempre filtra duplicatas, então podemos mandar esse slug 
        // toda vez que ela jogar. Se ela já tiver, o motor ignora.
        unlockedSlugs.Add("RELOGIOMUSICALPRIMEIRO_TICK");

        // ==========================================
        // REGRAS DE VELOCIDADE E ACERTOS (Em 60 Segundos)
        // ==========================================

        if (clockEvent.CorrectCount >= 3)
            unlockedSlugs.Add("RELOGIOMUSICALPONTUALIDADE_MUSICAL"); // 3 Acertos no minuto

        if (clockEvent.CorrectCount >= 5)
            unlockedSlugs.Add("RELOGIOMUSICALRITMO_CONSTANTE"); // 5 Acertos no minuto

        if (clockEvent.CorrectCount >= 8)
            unlockedSlugs.Add("RELOGIOMUSICALVELOCISTA_MUSICAL"); // 8 Acertos no minuto

        if (clockEvent.CorrectCount >= 10)
            unlockedSlugs.Add("RELOGIOMUSICALFLASH_MUSICAL"); // 10 Acertos no minuto (Insano!)

        if (clockEvent.CorrectCount >= 12)
            unlockedSlugs.Add("RELOGIOMUSICALCRONOMETRO_HUMANO"); // 12 Acertos no minuto (Máquina!)

        if (clockEvent.CorrectCount == 15)
            unlockedSlugs.Add("RELOGIOMUSICALDEUS_DO_TEMPO_MUSICAL"); // Gabaritou o Lote de 15 músicas no minuto!

        // ==========================================
        // REGRAS DE PRECISÃO (Sem Erros)
        // ==========================================

        // Se ela acertou pelo menos 5 músicas, mas não errou NENHUMA vez (não chutou/desistiu)
        if (clockEvent.CorrectCount >= 5 && clockEvent.IncorrectCount == 0)
        {
            unlockedSlugs.Add("RELOGIOMUSICALPERFECCIONISTA_TEMPORAL");
        }

        return unlockedSlugs;
    }
}