using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Rules;

public class QuizTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile player, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        if (eventContext is not QuizCompletedEvent quizEvent)
        {
            return unlockedSlugs;
        }

        // ==========================================
        // TENTATIVAS GERAIS
        // ==========================================
        if (player.QuizRuns == 1)
        {
            unlockedSlugs.Add("PRIMEIRAVEZPRIMEIRO_QUIZ");
            unlockedSlugs.Add("QUIZPRIMEIRA_PERGUNTA");
        }

        // ==========================================
        // ERROS
        // ==========================================
        if (!quizEvent.IsCorrect && player.QuizIncorrectAnswers == 1)
        {
            unlockedSlugs.Add("PRIMEIRAVEZPRIMEIRO_ERRO");
        }

        // ==========================================
        // ACERTOS
        // ==========================================
        if (quizEvent.IsCorrect)
        {
            switch (player.QuizCorrectAnswers)
            {
                case 5:
                    unlockedSlugs.Add("QUIZCURIOSO");
                    break;
                case 25:
                    unlockedSlugs.Add("QUIZENCICLOPEDIA_VIVA");
                    break;
                case 50:
                    unlockedSlugs.Add("QUIZGENIO_MUSICAL");
                    break;
                case 100:
                    unlockedSlugs.Add("QUIZCONSCIENCIA_MUSICAL_UNIVERSAL");
                    break;
            }
        }

        return unlockedSlugs;
    }
}