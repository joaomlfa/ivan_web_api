using IvanWeb.Application.Helpers;
using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Enums;
using IvanWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IvanWeb.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MusicalQuizController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public MusicalQuizController(ApplicationDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    [HttpGet("start")]
    public async Task<IActionResult> StartRound([FromQuery] Guid playerId, [FromQuery] MusicalGameMode mode)
    {
        var player = await _dbContext.PlayerProfiles
            .Include(p => p.GuessedSongs)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return NotFound("Perfil não encontrado.");

        // 1. CHECAGEM DE LIMITE DIÁRIO
        if (player.LastPlayedDate.Date < DateTime.UtcNow.Date)
        {
            // Virou o dia, reseta os contadores!
            player.Daily8BitRuns = 0;
            player.DailyHardRuns = 0;
            player.DailyClockRuns = 0;
            player.LastPlayedDate = DateTime.UtcNow;
        }

        if (mode == MusicalGameMode.EightBit && player.Daily8BitRuns >= 3)
            return BadRequest("Você já jogou o modo 8-bit 3x hoje! Volte amanhã.");
        if (mode == MusicalGameMode.Hard && player.DailyHardRuns >= 5)
            return BadRequest("Você já jogou o modo HARD 5x hoje! Volte amanhã.");
        if (mode == MusicalGameMode.RelogioMusical && player.DailyClockRuns >= 3)
            return BadRequest("Você já jogou o Relógio Musical 3x hoje! Volte amanhã.");

        // Incrementa a jogada do dia
        if (mode == MusicalGameMode.EightBit) player.Daily8BitRuns++;
        else if (mode == MusicalGameMode.Hard) player.DailyHardRuns++;
        else if (mode == MusicalGameMode.RelogioMusical) player.DailyClockRuns++;

        player.QuizSongsRuns++;
        await _dbContext.SaveChangesAsync();

        // 2. SISTEMA DE MÚSICAS INÉDITAS
        bool is8bit = mode == MusicalGameMode.EightBit;
        var guessedSongIds = player.GuessedSongs.Select(s => s.Id).ToList();

        // Busca uma música que tenha o Is8Bit correspondente e que NÃO esteja nas adivinhadas
        var song = await _dbContext.Songs
            .Where(s => s.Is8Bit == is8bit && !guessedSongIds.Contains(s.Id))
            .OrderBy(s => EF.Functions.Random())
            .FirstOrDefaultAsync();

        if (song == null)
            return NotFound("Uau! Você já adivinhou todas as músicas deste modo!");

        // 3. CRIAÇÃO DA SESSÃO SEGURA (Para o AudioController e o Anti-Cheat)
        string audioToken = Guid.NewGuid().ToString();
        var session = new AudioSession
        {
            SongId = song.Id,
            FileId = song.FileId,
            StartTime = DateTime.UtcNow
        };

        _cache.Set(audioToken, session, TimeSpan.FromMinutes(2));

        return Ok(new
        {
            AudioToken = audioToken,
            Points = song.Points,
            AudioUrl = $"/api/audio/stream?token={audioToken}"
        });
    }

    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer(
        [FromBody] AnswerMusicalRequest request,
        [FromServices] ITrophyEngineService trophyEngine)
    {
        // 1. RECUPERA E VALIDA A SESSÃO
        if (!_cache.TryGetValue(request.AudioToken, out AudioSession? session) || session == null)
            return BadRequest("Sessão inválida ou expirada (Demorou muito!).");

        var player = await _dbContext.PlayerProfiles.Include(p => p.GuessedSongs).FirstOrDefaultAsync(p => p.Id == request.PlayerId);
        var song = await _dbContext.Songs.FindAsync(session.SongId);

        if (player == null || song == null) return NotFound("Perfil ou música não encontrados.");

        // 2. CÁLCULO DE TEMPO ANTI-CHEAT
        var timeTakenSeconds = (DateTime.UtcNow - session.StartTime).TotalSeconds;

        // 3. VALIDAÇÃO FUZZY (A tolerância de erros de digitação que criamos)
        bool isCorrect = StringHelper.IsTolerableMatch(song.Name, request.Answer);
        int pointsEarned = isCorrect ? song.Points : 0;

        // 4. ATUALIZAÇÃO DO PLACAR E MÚSICAS INÉDITAS
        if (isCorrect)
        {
            player.QuizSongsCorrectAnswers++;
            player.TotalScore += pointsEarned;

            // Adiciona na lista para não ser sorteada novamente
            if (!player.GuessedSongs.Any(s => s.Id == song.Id))
                player.GuessedSongs.Add(song);
        }
        else
        {
            player.QuizSongsIncorrectAnswers++;
        }

        await _dbContext.SaveChangesAsync();

        // 5. MOTOR DE TROFÉUS (Passamos o tempo gasto no evento!)
        var quizEvent = new MusicalQuizCompletedEvent(
            songId: song.Id,
            isCorrect: isCorrect,
            timeTakenSeconds: timeTakenSeconds // O troféu "Velocidade Sonic" vai usar isso
        );
        var unlockedTrophies = await trophyEngine.ProcessTrophiesAsync(player.Id, quizEvent);

        // 6. FEEDBACK
        string? correctAnswerRevealed = (!isCorrect && request.GameMode != MusicalGameMode.EightBit) ? song.Name : null;

        // Destrói o token para evitar que ela envie a resposta duas vezes
        _cache.Remove(request.AudioToken);

        return Ok(new
        {
            Sucesso = true,
            Acertou = isCorrect,
            TempoGastoSegundos = Math.Round(timeTakenSeconds, 1),
            PontosGanhos = pointsEarned,
            RespostaCorreta = correctAnswerRevealed,
            TrofeusDesbloqueados = unlockedTrophies.Select(t => new { t.Name, t.Description, t.Icon })
        });
    }

    [HttpGet("clock-batch")]
    public async Task<IActionResult> StartClockBatch([FromQuery] Guid playerId)
    {
        var player = await _dbContext.PlayerProfiles.FindAsync(playerId);
        if (player == null) return NotFound("Perfil não encontrado.");

        // Regra do Limite Diário
        if (player.LastPlayedDate.Date < DateTime.UtcNow.Date)
        {
            player.Daily8BitRuns = 0;
            player.DailyHardRuns = 0;
            player.DailyClockRuns = 0;
            player.LastPlayedDate = DateTime.UtcNow;
        }

        if (player.DailyClockRuns >= 3)
            return BadRequest("Você já jogou o Relógio Musical 3x hoje! Volte amanhã.");

        // Consome a tentativa diária
        player.DailyClockRuns++;
        await _dbContext.SaveChangesAsync();

        // Pega 15 músicas aleatórias do banco
        var songs = await _dbContext.Songs
            .Where(s => !s.Is8Bit) // O Relógio Musical não usa músicas 8-bit
            .OrderBy(s => EF.Functions.Random())
            .Take(15)
            .ToListAsync();

        var batch = new List<object>();

        foreach (var song in songs)
        {
            string audioToken = Guid.NewGuid().ToString();
            var session = new AudioSession
            {
                SongId = song.Id,
                FileId = song.FileId,
                StartTime = DateTime.UtcNow
            };

            // Cache de 3 minutos é mais do que suficiente para um jogo de 1 minuto
            _cache.Set(audioToken, session, TimeSpan.FromMinutes(3));

            batch.Add(new
            {
                Id = song.Id,
                Name = song.Name, // O Flutter usará isso para validar a digitação localmente
                Points = song.Points,
                AudioUrl = $"/api/v1/audio/stream?token={audioToken}"
            });
        }

        return Ok(batch);
    }
    [HttpPost("clock-submit")]
    public async Task<IActionResult> SubmitClockBatch(
       [FromBody] SubmitClockRequest request,
       [FromServices] ITrophyEngineService trophyEngine)
    {
        var player = await _dbContext.PlayerProfiles.FindAsync(request.PlayerId);
        if (player == null) return NotFound("Perfil não encontrado.");

        player.TotalScore += request.PointsEarned;
        player.QuizSongsRuns++;
        player.QuizSongsCorrectAnswers += request.CorrectCount;
        player.QuizSongsIncorrectAnswers += request.IncorrectCount;

        await _dbContext.SaveChangesAsync();

        // Cria o Evento do Relógio com os acertos e erros da rodada!
        var clockEvent = new ClockCompletedEvent(request.CorrectCount, request.IncorrectCount);

        // O motor processa e devolve a lista de troféus que ela ganhou agorinha!
        var unlockedTrophies = await trophyEngine.ProcessTrophiesAsync(player.Id, clockEvent);

        return Ok(new
        {
            Sucesso = true,
            PontosGanhos = request.PointsEarned,
            // Converte os troféus para o formato que o Flutter espera exibir no Toast
            TrofeusDesbloqueados = unlockedTrophies.Select(t => new { t.Name, t.Description, t.Icon })
        });
    }
}

public class SubmitClockRequest
{
    public Guid PlayerId { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public int PointsEarned { get; set; }
}

public class AnswerMusicalRequest
{
    public Guid PlayerId { get; set; }
    public string AudioToken { get; set; } = string.Empty; // <-- Agora a resposta exige o Token da sessão
    public string Answer { get; set; } = string.Empty;
    public MusicalGameMode GameMode { get; set; }
}

public class AudioSession
{
    public Guid SongId { get; set; }
    public string FileId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}