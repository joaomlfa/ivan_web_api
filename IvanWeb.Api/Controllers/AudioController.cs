using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IvanWeb.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AudioController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _env;

    // Injetamos o Cache e o Environment (que sabe onde a API está instalada no servidor)
    public AudioController(IMemoryCache cache, IWebHostEnvironment env)
    {
        _cache = cache;
        _env = env;
    }

    [HttpGet("stream")] // Rota: GET /api/audio/stream?token=xyz
    public IActionResult StreamAudio([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token não fornecido.");

        // 1. Tenta achar o token no cache. Se achar, extrai o FileId (ex: "1")
        if (!_cache.TryGetValue(token, out AudioSession? session) || session == null)
        {
            return Unauthorized("Token inválido ou expirado.");
        }
        string fileId = session.FileId;
        // 2. Monta o caminho para a pasta secreta Storage/Songs
        // O ContentRootPath é a pasta raiz do projeto IvanWeb.Api
        string songsFolder = Path.Combine(_env.ContentRootPath, "Storage", "Songs");
        string filePath = Path.Combine(songsFolder, $"{fileId}.mp3");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Arquivo de áudio não encontrado no servidor.");
        }

        // 3. A Mágica do Streaming Web!
        // enableRangeProcessing: true -> É OBRIGATÓRIO! Isso permite que o navegador 
        // faça o streaming aos poucos e consiga tocar a música nativamente.
        return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
    }
}