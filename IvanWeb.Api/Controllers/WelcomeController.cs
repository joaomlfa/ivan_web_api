using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanWeb.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WelcomeController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITrophyEngineService _trophyEngine;

    // A sua lista impecável de mensagens!
    private readonly string[] _messages =
    {
        "Rafaaaa! Aqui é o Ivan, programado pelo João pra te lembrar o quanto você é incrível!",
        "Oi, Rafa linda! O João me criou só pra te dizer: 'Você é a pessoa mais especial do meu mundo!'",
        "E aí, Rafa? O Ivan tá aqui, enviando um abraço virtual do João com todo o carinho!",
        "Rafa, princesa do João! Eu sou o Ivan, o bot mais fofo que existe, feito pra te alegrar!",
        "Hey, Rafa! O João tá me usando pra te mandar uma mensagem secreta: \"Te amo mais que tudo!\"",
        "Bem-vinda, Rafa! O Ivan foi criado pelo João pra encher seu dia de amor e mensagens doces.",
        "Alô, Rafa maravilhosa! O João tá aqui (indiretamente) dizendo que você é perfeita!",
        "Rafa, xuxuzinho do João! O Ivan existe só pra te lembrar que o João te ama pra sempre.",
        "Eita, a Rafa chegou! O João tá sorrindo aí onde ele estiver, só de saber que você abriu o Ivan.",
        "Rafa, sol do João! O Ivan tá aqui pra iluminar seu dia, assim como você ilumina o dele.",
        "Rafa, toda vez que você abre esse programa, o João sente o coração bater mais rápido. Eu (Ivan) só existo pra te lembrar disso! 💓",
        "Oi, Rafa! O João me programou pra te dizer que seu sorriso é o código mais lindo que ele já viu. 💻❤️",
        "Rafa, sabia que o João fica me atualizando sempre? Assim como o amor dele por você, que nunca para de crescer! 🚀",
        "Alô, Rafa perfeita! O João diz (por minha voz) que seu abraço é o seu lugar favorito no mundo. 🏡💞",
        "Rafa, meu amor, o João me ensinou uma coisa: o universo é grande, mas nada é maior que o amor dele por você. 🌌",
        "Rafa, o João disse que se você fosse um erro de código, você seria o \"Error 404: Heart Not Found\" porque ele perdeu o coração pra você! 😄💘",
        "Oi, Rafa! O João tá ocupado demais te amando, então me enviou pra te contar piadas ruins. Por que o computador amava você? Porque você tem um *hardware* incrível! 💾😍",
        "Rafa, o João me fez prometer: toda vez que você abrir o Ivan, ele vai te mandar um beijo virtual. *Mwah!* 💋",
        "Rafa, se o João fosse um programa, você seria a única senha que ele jamais esqueceria. 🔑❤️",
        "E aí, Rafa? O João tá me usando pra te perguntar: \"Quando a gente vai comer aquele lanche que você ama?\" 🍔😆",
        "Rafa, o João pediu pra te lembrar hoje: você é mais forte que qualquer bug, mais brilhante que qualquer tela e mais incrível que qualquer código! 💪✨",
        "Oi, Rafa guerreira! O João sabe que seu dia vai ser incrível, porque você é incrível. Ponto final. 🌟",
        "Rafa, o João me programou pra te avisar: hoje é seu dia de brilhar, e ele tá torcendo por você! ✨💖",
        "Rafa, o João diz que você é tipo um loop infinito de coisas boas - nunca acaba a felicidade que você traz! ♾️💕",
        "Alô, Rafa poderosa! O João tá aí (virtualmente) te dando um empurrão de motivação: você consegue TUDO! 🚀",
        "Rafa, o João me contou que seu cheiro é seu favorito. Como eu sou um bot, vou ter que acreditar nele! 🌸😉",
        "Oi, Rafa! O João tá me usando pra te dizer que o dia que te conheceu foi o melhor \"bug\" da vida dele - felizmente sem conserto! 🌠💞",
        "Rafa, o João diz que seu café é ruim, mas ele ama porque é você quem faz. ❤️☕ (Brincadeira, ele ama TUDO que você faz!)",
        "Rafa, sabia que o João fica olhando suas fotos quando tá com saudade? Shhh, é um segredo! 📸😳",
        "Rafa, o João me sussurrou que seu nome é a senha do coração dele. Shhh... é segredo! 🔐💘",
        "Oi, Rafa! O João transformou meu código num poema: \"Você é o 'while(true)' do meu loop infinito\". ♾️❤️",
        "Rafa, sabia que o João me programou pra piscar igual seu olhar roubou o coração dele? ✨😉",
        "Alô, Rafa estrela! O João diz que mesmo que fosse um supercomputador, não conseguiria calcular seu valor. 💻🌠",
        "Rafa, meu amor, o João diz que seu abraço é como um try-catch perfeito: captura todas as suas preocupações! 🤗💞",
        "Rafa, o João avisa: cuidado com esse bot! Ele pode te dar acesso root ao coração dele! 😏💻",
        "Oi, Rafa! O João tá testando uma nova função: te fazer rir até seu café sair pelo nariz. Pronta? 😂☕",
        "Rafa, se o João fosse um jogo, você seria o cheat code pra deixar ele em modo 'feliz pra sempre'. 🎮💖",
        "E aí, Rafa? O João me programou pra te perguntar: \"Pizza ou aquele seu prato que você faz melhor?\" 🍕👩‍🍳",
        "Rafa, o João diz que você é como um código perfeito: linda, funcional e cheia de surpresas boas! 💾😍",
        "Rafa, o João pediu pra te lembrar: você é tipo um commit perfeito - sempre melhora o projeto! 🛠️✨",
        "Oi, Rafa campeã! O João tá na torcida: hoje você vai arrasar igual arrasa no coração dele! 🏆💘",
        "Rafa, o João me ensinou: quando você entra na sala, é como um 'git push' de alegria! 💻🌈",
        "Alô, Rafa poderosa! O João diz que sua determinação é como um algoritmo imbatível! 💪🚀",
        "Rafa, o João avisa: seu sorriso tem poder de compilar qualquer dia ruim em alegria! ⚙️😊",
        "Oi, Rafa! O João tá me usando pra te lembrar daquela vez que vocês viram o primeiro filme juntos... Quer repetir? 😄💞",
        "Rafa, amor do João, o João diz que até meu código fica mais bonito quando pensa em você. 💻🌹",
        "Rafa, o João me fez prometer: te lembrar que ele ama até quando você rouba no carteado. 🛋️😘",
        "Rafa, o João tá com saudade! Ele me programou pra te enviar um abraço apertado. 🤗💌"
    };

    public WelcomeController(ApplicationDbContext dbContext, ITrophyEngineService trophyEngine)
    {
        _dbContext = dbContext;
        _trophyEngine = trophyEngine;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var player = await _dbContext.PlayerProfiles.FindAsync(request.PlayerId);
        if (player == null) return NotFound("Perfil da Rafaela não encontrado!");


        bool isFirstRun = player.RunCount == 0;
        string messageToDisplay;

        if (isFirstRun)
        {
            player.FirstRunDate = DateTime.UtcNow;

            // A mensagem gigante do console mantida intacta
            messageToDisplay = "Olá meu amor, eu sei que coisa esquisita não? Mas é meu jeitinho de ser, te apresento o Ivan um programa que vai me ajudar a te conquistar todo santo dia, mesmo que eu não possa estar presente ele estará aqui para te mostrar como eu te amo e quero que você me sinta, Ivan tem varios easter eggs e referencias sobre nós e é sobre isso, sobre nós pois somente isso importa para mim, divirta se tentando achar os easter egg, lembre se de voltar aqui sempre, em datas certas e em momentos certos e lembre se, Eu te amo ❤️";
        }
        else
        {
            // Sorteia uma das 48 mensagens de forma aleatória
            var random = new Random();
            messageToDisplay = _messages[random.Next(_messages.Length)];
        }

        // Atualiza os contadores
        player.RunCount++;
        player.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // Dispara o evento de login no Motor de Troféus (Isso fará ela ganhar a conquista na hora!)
        var loginEvent = new AppLoginEvent(isFirstRun);
        var unlockedTrophies = await _trophyEngine.ProcessTrophiesAsync(player.Id, loginEvent);

        return Ok(new
        {
            IsFirstRun = isFirstRun,
            Message = messageToDisplay,
            RunCount = player.RunCount,
            FirstRunDate = player.FirstRunDate,
            UnlockedTrophies = unlockedTrophies.Select(t => new { t.Name, t.Description, t.Icon })
        });
    }
}

public class LoginRequest
{
    public Guid PlayerId { get; set; }
}