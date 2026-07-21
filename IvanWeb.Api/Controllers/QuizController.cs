using IvanWeb.Application.Helpers;
using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Enums;
using IvanWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanWeb.Api.Controllers;

// 1. DTO: É exatamente esse formato JSON que o Flutter vai mandar num POST
public class AnswerQuizRequest
{
    public Guid PlayerId { get; set; }
    public Guid QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}

[ApiController]
[Route("api/v1/[controller]")] // A rota ficará: /api/quiz
public class QuizController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITrophyEngineService _trophyEngine;

    // Injetamos o Banco de Dados e o nosso novo Motor de Troféus
    public QuizController(ApplicationDbContext dbContext, ITrophyEngineService trophyEngine)
    {
        _dbContext = dbContext;
        _trophyEngine = trophyEngine;
    }

    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] AnswerQuizRequest request)
    {
        var player = await _dbContext.PlayerProfiles.FindAsync(request.PlayerId);
        if (player == null) return NotFound("Perfil não encontrado.");

        var question = await _dbContext.QuizQuestions.FindAsync(request.QuestionId);
        if (question == null) return NotFound("Pergunta não encontrada.");

        if (question.IsAnswered) return BadRequest("Essa pergunta já foi respondida.");

        // A Mágica da Tolerância de Erros
        bool isCorrect = StringHelper.IsTolerableMatch(question.Answer, request.Answer);

        int pointsEarned = isCorrect ? question.Points : 0;

        // Atualiza o perfil
        player.QuizRuns++;
        player.LastQuizPlayedDate = DateTime.UtcNow;

        if (isCorrect)
        {
            player.QuizCorrectAnswers++;
            player.TotalScore += pointsEarned;
            question.IsAnswered = true;

        }
        else
        {
            player.QuizIncorrectAnswers++;
            question.IsAnswered = false;

        }


        await _dbContext.SaveChangesAsync();

        var quizEvent = new QuizCompletedEvent(
            questionId: request.QuestionId,
            isCorrect: isCorrect,
            type: QuestionType.Personal,
            pointsEarned: pointsEarned
        );

        var unlockedTrophies = await _trophyEngine.ProcessTrophiesAsync(player.Id, quizEvent);

        return Ok(new
        {
            Sucesso = true,
            Acertou = isCorrect,
            Mensagem = isCorrect ? "Certa Resposta!" : $"Ah, quase!",
            PontosGanhos = pointsEarned,
            TrofeusDesbloqueados = unlockedTrophies.Select(t => new { t.Name, t.Description, t.Icon, t.Rarity })
        });
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyQuiz()
    {
        var player = await _dbContext.PlayerProfiles.FirstOrDefaultAsync();
        if (player == null) return NotFound("Perfil não encontrado.");

        // Pega o fuso horário do Brasil
        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        // Converte a data do último quiz para o horário do Brasil
        var lastQuizBrazil = TimeZoneInfo.ConvertTimeFromUtc(player.LastQuizPlayedDate, brazilTimeZone);

        // Converte o momento de AGORA para o horário do Brasil
        var nowBrazil = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

        // 1. Verifica se ela já respondeu hoje (reset à meia-noite)
        bool alreadyAnsweredToday = player.LastPlayedDate.Date == DateTime.UtcNow.Date && player.QuizRuns > 0; // Ajuste a lógica de flag diária conforme preferir

        if (alreadyAnsweredToday)
        {
            return Ok(new { AlreadyAnsweredToday = true });
        }

        // 2. Busca uma pergunta aleatória que ainda não foi respondida
        var nextQuestion = await _dbContext.QuizQuestions
            .Where(q => !q.IsAnswered)
            .OrderBy(q => Guid.NewGuid()) // Sorteio aleatório no banco
            .FirstOrDefaultAsync();

        if (nextQuestion == null)
        {
            return Ok(new { NoMoreQuestions = true, Message = "Você já respondeu todas as perguntas do banco!" });
        }

        // 3. Retorna para o Front-end
        return Ok(new
        {
            Id = nextQuestion.Id,
            Question = nextQuestion.Question,
            AlreadyAnsweredToday = false
            // Se formos usar múltiplas escolhas, enviaríamos as "Options" aqui
        });
    }
}