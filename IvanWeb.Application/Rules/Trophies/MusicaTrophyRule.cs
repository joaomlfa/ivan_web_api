using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;
using IvanWeb.Domain.Enums;

namespace IvanWeb.Application.Rules;

public class MusicaTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile player, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        if (eventContext is not MusicalQuizCompletedEvent musicalEvent)
            return unlockedSlugs;

        // ==========================================
        // TENTATIVAS GERAIS
        // ==========================================
        if (player.QuizSongsRuns == 1)
        {
            unlockedSlugs.Add("PRIMEIRAVEZPRIMEIRAS_MUSICAS");
            unlockedSlugs.Add("MUSICASPRIMEIRA_NOTA");
        }

        // ==========================================
        // ERROS
        // ==========================================
        if (!musicalEvent.IsCorrect && player.QuizSongsIncorrectAnswers == 1)
        {
            // O troféu genérico de erro, caso ainda não tenha ganho no quiz de texto
            unlockedSlugs.Add("PRIMEIRAVEZPRIMEIRO_ERRO");
        }

        // ==========================================
        // ACERTOS E VELOCIDADE
        // ==========================================
        if (musicalEvent.IsCorrect)
        {
            if (player.QuizSongsCorrectAnswers == 1)
                unlockedSlugs.Add("PRIMEIRAVEZPRIMEIRO_ACERTO_MUSICAL");

            // Troféu de velocidade: Acertou em menos de 3 segundos
            if (musicalEvent.TimeTakenSeconds <= 3.0)
                unlockedSlugs.Add("MUSICASVELOCIDADE_SONIC");

            // Acertos Totais
            switch (player.QuizSongsCorrectAnswers)
            {
                case 25:
                    unlockedSlugs.Add("MUSICASMELOMANO");
                    break;
                case 50:
                    unlockedSlugs.Add("MUSICASCONHECEDOR_PROFUNDO");
                    break;
                case 100:
                    unlockedSlugs.Add("MUSICASDEUS_DA_MUSICA");
                    break;
            }
        }

        return unlockedSlugs;
    }
}