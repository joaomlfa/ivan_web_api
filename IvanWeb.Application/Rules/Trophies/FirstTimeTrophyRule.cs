using IvanWeb.Application.Interfaces;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Rules.Trophies;

public class FirstTimeTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile profile, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        // Aqui nós checamos o tipo de evento que chegou. 
        // Você criaria essas classes de evento (ex: QuizCompletedEvent) na pasta Models
        if (eventContext is Models.Events.AppStartedEvent && profile.QuizRuns == 0 && profile.Purchases.Count == 0)
        {
            unlockedSlugs.Add("PRIMEIRO_PASSO");
        }
        else if (eventContext is Models.Events.QuizCompletedEvent quizEvent)
        {
            if (profile.QuizRuns == 1) unlockedSlugs.Add("PRIMEIRO_QUIZ");

            // Baseado na resposta do Quiz
            if (!quizEvent.IsCorrect && profile.QuizIncorrectAnswers == 1)
            {
                unlockedSlugs.Add("PRIMEIRO_ERRO");
            }
        }
        else if (eventContext is Models.Events.StorePurchaseEvent)
        {
            if (profile.Purchases.Count == 1) unlockedSlugs.Add("PRIMEIRA_COMPRA");
        }

        return unlockedSlugs;
    }
}