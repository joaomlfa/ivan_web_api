using IvanWeb.Domain.Entities;
using IvanWeb.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace IvanWeb.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Aplica migrations pendentes automaticamente (ótimo para não ter que rodar comando toda hora)
        await context.Database.MigrateAsync();

        // Só insere se a tabela de troféus estiver vazia
        if (!await context.Trophies.AnyAsync())
        {
            var trofeus = GetDefaultTrophies();
            await context.Trophies.AddRangeAsync(trofeus);
            await context.SaveChangesAsync();
        }

        // Só insere se a tabela de músicas estiver vazia
        if (!await context.Songs.AnyAsync())
        {
            await GetDefaultSongs(context);
        }

        if (!await context.QuizQuestions.AnyAsync())
        {
            await GetDefaultQuizQuestions(context);

        }

        if (!await context.StoreItems.AnyAsync())
        {
            var itensLoja = new List<StoreItem>
            {
                new StoreItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Desbloquear Sala de Troféus",
                    Icon = "🏆",
                    Price = 50,
                    IsSinglePurchase = true,
                    SendWhatsApp = false
                },
                new StoreItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Caixa de Chocolate",
                    Icon = "🍫",
                    Price = 100,
                    IsSinglePurchase = false,
                    SendWhatsApp = true // Esse cobra no Zap!
                },
                new StoreItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Cartão personalizado",
                    Icon = "🎁",
                    Price = 70,
                    IsSinglePurchase = false,
                    SendWhatsApp = true // Esse também!
                }
            };

            context.StoreItems.AddRange(itensLoja);
            await context.SaveChangesAsync();
        }

        if (!await context.PlayerProfiles.AnyAsync())
        {
            var rafaela = new PlayerProfile
            {
                Id = Guid.Parse("05151fad-3623-4446-9937-a2bc9403e672"),
                Name = "Rafaela",
                TotalScore = 0,
                RunCount = 0,
            };
            context.PlayerProfiles.Add(rafaela);
            await context.SaveChangesAsync();
        }
        else
        {
            var existingPlayer = await context.PlayerProfiles.FirstOrDefaultAsync();
            if (existingPlayer != null && existingPlayer.Id != Guid.Parse("05151fad-3623-4446-9937-a2bc9403e672"))
            {
                // Deleta e recria para forçar o GUID correto
                context.PlayerProfiles.Remove(existingPlayer);
                await context.SaveChangesAsync();

                var rafaela = new PlayerProfile
                {
                    Id = Guid.Parse("05151fad-3623-4446-9937-a2bc9403e672"),
                    Name = "Rafaela",
                    TotalScore = existingPlayer.TotalScore,
                    RunCount = existingPlayer.RunCount,
                };
                context.PlayerProfiles.Add(rafaela);
                await context.SaveChangesAsync();
            }
        }
    }

    public static List<Trophy> GetDefaultTrophies()
    {
        var trofeus = new List<Trophy>();



        trofeus.Add(CreateTrophy("Primeiro Passo", "Bem-vindo ao mundo de Ivan! Você deu seu primeiro passo.", "Execute o jogo pela primeira vez", "👶", 10, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Primeiras Músicas", "Você ouviu sua primeira música no jogo!", "Toque qualquer música pela primeira vez", "🎵", 15, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Primeiro Quiz", "Conhecimento é poder! Você jogou seu primeiro quiz.", "Complete seu primeiro quiz", "❓", 20, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Primeira Compra", "Gastando os primeiros pontos na loja!", "Compre qualquer item na loja", "💰", 25, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Primeiro Acerto Musical", "Você tem bom ouvido! Acertou sua primeira música.", "Acerte uma música no 'Qual é a Música'", "🎯", 30, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Primeiro Erro", "Errar é humano! Todo mundo erra no começo.", "Erre uma pergunta", "❌", 5, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Explorador", "Curiosidade mata o gato! Você visitou todos os menus.", "Visite todos os menus principais pelo menos uma vez", "🧭", 35, TrophyCategory.PrimeiraVez, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Primeiro Relógio", "Tempo é ouro! Você jogou seu primeiro Relógio Musical.", "Complete uma sessão do Relógio Musical", "⏰", 40, TrophyCategory.PrimeiraVez, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Primeira Configuração", "Personalizando sua experiência!", "Altere qualquer configuração do jogo", "⚙️", 15, TrophyCategory.PrimeiraVez, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Bem-vindo à Família", "Você é oficialmente parte da família Ivan!", "Complete sua primeira semana jogando", "👨‍👩‍👧‍👦", 100, TrophyCategory.PrimeiraVez, TrophyRarity.Epic));

        // CATEGORIA: PONTUAÇÃO (15 troféus)

        trofeus.Add(CreateTrophy("Primeiros Pontos", "Todo grande jogador começou assim!", "Alcance 100 pontos totais", "📈", 50, TrophyCategory.Pontuacao, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Escalando", "Você está pegando o jeito!", "Alcance 500 pontos totais", "🏃", 75, TrophyCategory.Pontuacao, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Milhar", "Mil pontos! Você está indo bem!", "Alcance 1.000 pontos totais", "1️⃣", 100, TrophyCategory.Pontuacao, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Cinco Mil", "5000 pontos! Você é dedicado!", "Alcance 5.000 pontos totais", "5️⃣", 200, TrophyCategory.Pontuacao, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Dez Mil", "10.000 pontos! Você é um mestre!", "Alcance 10.000 pontos totais", "🔟", 300, TrophyCategory.Pontuacao, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Vinte e Cinco Mil", "25.000 pontos! Impressionante!", "Alcance 25.000 pontos totais", "🏆", 500, TrophyCategory.Pontuacao, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Cinquenta Mil", "50.000 pontos! Você é uma lenda!", "Alcance 50.000 pontos totais", "👑", 750, TrophyCategory.Pontuacao, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Cem Mil", "100.000 pontos! Mestre dos mestres!", "Alcance 100.000 pontos totais", "💎", 1000, TrophyCategory.Pontuacao, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Duzentos e Cinquenta Mil", "250.000 pontos! Você quebrou o jogo!", "Alcance 250.000 pontos totais", "🌟", 2000, TrophyCategory.Pontuacao, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Meio Milhão", "500.000 pontos! Deus do jogo!", "Alcance 500.000 pontos totais", "⭐", 5000, TrophyCategory.Pontuacao, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Um Milhão", "1.000.000 de pontos! IMPOSSÍVEL!", "Alcance 1.000.000 pontos totais", "🌠", 10000, TrophyCategory.Pontuacao, TrophyRarity.Legendary, true, "Será que alguém consegue chegar tão longe?"));
        trofeus.Add(CreateTrophy("Sequência Inicial", "Três acertos seguidos!", "Acerte 3 perguntas/músicas consecutivas", "🎯", 50, TrophyCategory.Pontuacao, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Sequência Impressionante", "Cinco acertos seguidos!", "Acerte 5 perguntas/músicas consecutivas", "🔥", 100, TrophyCategory.Pontuacao, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Sequência Lendária", "Dez acertos seguidos!", "Acerte 10 perguntas/músicas consecutivas", "⚡", 250, TrophyCategory.Pontuacao, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Perfeição", "Sequência perfeita de 20!", "Acerte 20 perguntas/músicas consecutivas", "💯", 1000, TrophyCategory.Pontuacao, TrophyRarity.Legendary));

        // CATEGORIA: MÚSICAS - TROFÉUS CRIATIVOS (25 troféus - IDs 26-50)

        // 🌟 BÁSICOS/INICIANTES (Common - IDs 26-30)
        trofeus.Add(CreateTrophy("Primeira Nota", "Sua jornada musical começou! Primeira vez abrindo o jogo musical.", "Abra o jogo musical uma vez", "🎵", 25, TrophyCategory.Musicas, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Ouvido Musical", "Você tem bom ouvido! Sequência impressionante.", "Acerte 5 músicas consecutivas", "👂", 50, TrophyCategory.Musicas, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Ritmo na Veia", "O ritmo está no seu sangue! Sessão produtiva.", "Acerte 10 músicas em uma sessão", "💓", 75, TrophyCategory.Musicas, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Descobrindo Sons", "Versatilidade musical! Você explora diferentes estilos.", "Acerte músicas de 3 gêneros diferentes", "🔍", 100, TrophyCategory.Musicas, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Velocidade Sonic", "Reflexos de ninja! Identificação instantânea.", "Acerte uma música em menos de 3 segundos", "⚡", 125, TrophyCategory.Musicas, TrophyRarity.Common));

        // 🔥 INTERMEDIÁRIOS (Rare - IDs 31-40)
        trofeus.Add(CreateTrophy("Melómano", "Verdadeiro amante da música! Conhecimento sólido.", "Acerte 25 músicas no total", "🎧", 200, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Expert em 8-bit", "Nostálgico dos videogames! Mestre dos clássicos digitais.", "Acerte 10 músicas de videogame seguidas", "🎮", 300, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Nostálgico", "Guardião das memórias musicais! Conhece os clássicos.", "Acerte 15 músicas clássicas/antigas", "📻", 250, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Rock Star", "O rock corre nas suas veias! Atitude e energia.", "Acerte 20 músicas de rock", "🎸", 275, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Dançarino", "A pista te chama! Especialista em batidas eletrônicas.", "Acerte 15 músicas eletrônicas/dance", "💃", 225, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Sertanejo Raiz", "Do coração do Brasil! Conhece a música do campo.", "Acerte 12 músicas sertanejas", "🤠", 200, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("MPB no Coração", "A alma brasileira em cada nota! Cultura musical nacional.", "Acerte 10 músicas de MPB", "🇧🇷", 250, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Internacional", "Cidadão do mundo! Sem fronteiras musicais.", "Acerte 8 músicas em inglês", "🌍", 200, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Década Perdida", "Viajante no tempo! Expert dos anos dourados.", "Acerte 15 músicas dos anos 80/90", "📼", 300, TrophyCategory.Musicas, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Hit Parade", "Antenado com o presente! Conhece os sucessos atuais.", "Acerte 20 sucessos atuais", "📈", 275, TrophyCategory.Musicas, TrophyRarity.Rare));

        // ⚡ AVANÇADOS (Epic - IDs 41-48)
        trofeus.Add(CreateTrophy("Mestre dos Gêneros", "Conhecimento universal! Domina todos os estilos musicais.", "Acerte músicas de 10 gêneros diferentes", "🎭", 500, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Sequência Lendária", "Consistência impressionante! Uma maratona de acertos.", "Acerte 30 músicas seguidas", "🔥", 750, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Speed Run Musical", "Reflexos sobre-humanos! Identificação em tempo recorde.", "Acerte 10 músicas em menos de 2 segundos cada", "⚡", 600, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Conhecedor Profundo", "Biblioteca musical viva! Vasto conhecimento acumulado.", "Acerte 50 músicas no total", "📚", 500, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Radar Musical", "Caçador de raridades! Encontra agulhas no palheiro.", "Acerte 5 músicas com menos de 30% de acerto geral", "📡", 800, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Maratonista", "Resistência musical! Uma sessão épica de descobertas.", "Acerte 25 músicas em uma única sessão", "🏃‍♂️", 600, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Perfeccionista", "Precisão cirúrgica! Qualidade acima de quantidade.", "Mantenha 95%+ de acerto em 20 músicas", "🎯", 700, TrophyCategory.Musicas, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Caçador de Raridades", "Indiana Jones da música! Descobre tesouros escondidos.", "Acerte 5 músicas secretas/easter eggs", "🕵️", 1000, TrophyCategory.Musicas, TrophyRarity.Epic));

        // 👑 LENDÁRIOS (Legendary - IDs 49-50)
        trofeus.Add(CreateTrophy("Deus da Música", "Divindade musical! Conhecimento transcendental.", "Acerte 100 músicas no total", "👑", 2000, TrophyCategory.Musicas, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Arquivo Musical Vivo", "Guardião da história musical! Memória de todas as eras.", "Acerte pelo menos 1 música de cada década (60s-2020s)", "📜", 2500, TrophyCategory.Musicas, TrophyRarity.Legendary));

        // CATEGORIA: QUIZ - TROFÉUS CRIATIVOS (20 troféus - IDs 51-70)

        // 🌟 BÁSICOS (Common - IDs 51-55)
        trofeus.Add(CreateTrophy("Primeira Pergunta", "O conhecimento começa com uma pergunta!", "Complete seu primeiro quiz", "❓", 25, TrophyCategory.Quiz, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Curioso", "A curiosidade não matou o gato... o fez mais inteligente!", "Acerte 5 questões de quiz", "🤔", 50, TrophyCategory.Quiz, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Estudioso", "O saber não ocupa lugar... mas dá pontos!", "Complete 3 quizzes completos", "📚", 75, TrophyCategory.Quiz, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Mente Ágil", "Pensamento rápido e certeiro!", "Acerte 3 questões em menos de 5 segundos cada", "🧠", 100, TrophyCategory.Quiz, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Sequência de Saber", "Conhecimento em cadeia!", "Acerte 5 questões consecutivas", "🔗", 125, TrophyCategory.Quiz, TrophyRarity.Common));

        // 🔥 INTERMEDIÁRIOS (Rare - IDs 56-65)
        trofeus.Add(CreateTrophy("Enciclopédia Viva", "Uma biblioteca ambulante de conhecimento musical!", "Acerte 25 questões de quiz no total", "📖", 200, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Expert em Curiosidades", "Sabe até o que não devia saber!", "Acerte questões sobre 5 artistas diferentes", "🎭", 250, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Historiador Musical", "Guardião das memórias da música!", "Acerte 10 questões sobre música antiga", "📜", 300, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Técnico em Música", "Conhece os bastidores da criação musical!", "Acerte questões sobre instrumentos e produção", "🎚️", 275, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Crítico Musical", "Análise profunda e conhecimento refinado!", "Acerte questões sobre teoria musical", "🎯", 350, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Maratonista Mental", "Resistência intelectual impressionante!", "Complete 10 quizzes em sequência", "🏃‍♂️", 400, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Perfeccionista do Saber", "Precisão cirúrgica nas respostas!", "Complete um quiz com 100% de acerto", "💯", 500, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Especialista em Gêneros", "Domina o conhecimento de todos os estilos!", "Acerte questões sobre 8 gêneros musicais diferentes", "🎪", 450, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Radar de Talentos", "Conhece artistas famosos e emergentes!", "Acerte questões sobre 15 artistas diferentes", "🌟", 375, TrophyCategory.Quiz, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Velocidade do Pensamento", "Einstein ficaria orgulhoso!", "Acerte 10 questões em menos de 3 segundos cada", "⚡", 600, TrophyCategory.Quiz, TrophyRarity.Rare));

        // ⚡ AVANÇADOS (Epic - IDs 66-70)
        trofeus.Add(CreateTrophy("Gênio Musical", "QI musical fora da curva!", "Acerte 50 questões de quiz no total", "🧙‍♂️", 750, TrophyCategory.Quiz, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Mestre dos Mistérios", "Desvenda os segredos mais profundos da música!", "Acerte questões sobre curiosidades raras", "🔍", 800, TrophyCategory.Quiz, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Supercomputador Musical", "Processamento de dados musical instantâneo!", "Complete 20 quizzes com mais de 90% de acerto", "💻", 1000, TrophyCategory.Quiz, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Oráculo da Música", "Sabe muito sobre nós!", "Acerte 10 questões sobre nosso relacionamento", "🔮", 1200, TrophyCategory.Quiz, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Consciência Musical Universal", "Conhecimento transcendental sobre toda a música!", "Acerte 100 questões de quiz no total", "🌌", 2000, TrophyCategory.Quiz, TrophyRarity.Legendary));

        // CATEGORIA: RELÓGIO MUSICAL - TROFÉUS CRIATIVOS (15 troféus - IDs 71-85)

        // 🌟 BÁSICOS (Common - IDs 71-75)
        trofeus.Add(CreateTrophy("Primeiro Tick", "O tempo começou a contar para você!", "Complete sua primeira sessão do Relógio Musical", "⏰", 30, TrophyCategory.RelogioMusical, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Pontualidade Musical", "Chegou na hora certa! Sempre no ritmo.", "Complete 3 sessões do Relógio Musical", "⏱️", 60, TrophyCategory.RelogioMusical, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Ritmo Constante", "Como um metrônomo, sempre consistente!", "Complete 5 sessões do Relógio Musical", "🎼", 90, TrophyCategory.RelogioMusical, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Velocista Musical", "Rapidez que impressiona! Som na velocidade da luz.", "Complete uma sessão em menos de 2 minutos", "💨", 120, TrophyCategory.RelogioMusical, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Maratonista do Tempo", "Resistência temporal impressionante!", "Complete uma sessão de mais de 10 minutos", "🏃", 150, TrophyCategory.RelogioMusical, TrophyRarity.Common));

        // 🔥 INTERMEDIÁRIOS (Rare - IDs 76-82)
        trofeus.Add(CreateTrophy("Mestre do Compasso", "Domina o tempo como um maestro!", "Complete 10 sessões do Relógio Musical", "🎭", 250, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Cronômetro Humano", "Precisão temporal de relógio suíço!", "Acerte o tempo de 10 músicas consecutivas", "⏳", 300, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Guardião do Tempo", "O tempo dobra-se à sua vontade musical!", "Complete 15 sessões do Relógio Musical", "🕰️", 400, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Flash Musical", "Mais rápido que a própria música!", "Complete 5 sessões em menos de 1 minuto cada", "⚡", 500, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Perfeccionista Temporal", "Cada segundo conta, cada batida importa!", "Complete uma sessão com 100% de precisão", "🎯", 450, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Viajante do Tempo", "Navega pelas épocas musicais com facilidade!", "Complete sessões com músicas de 5 décadas diferentes", "🌀", 600, TrophyCategory.RelogioMusical, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Senhor dos Segundos", "Cada tick do relógio obedece ao seu comando!", "Complete 20 sessões do Relógio Musical", "👑", 750, TrophyCategory.RelogioMusical, TrophyRarity.Rare));

        // ⚡ AVANÇADOS (Epic - IDs 83-85)
        trofeus.Add(CreateTrophy("Deus do Tempo Musical", "Transcendeu as limitações temporais!", "Complete 30 sessões do Relógio Musical", "⚡", 1000, TrophyCategory.RelogioMusical, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Eternidade Musical", "O tempo pára quando você joga!", "Mantenha 90%+ de precisão em 15 sessões", "♾️", 1500, TrophyCategory.RelogioMusical, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Arquiteto do Tempo", "Construiu sua própria dimensão temporal musical!", "Complete 50 sessões do Relógio Musical", "🏗️", 2500, TrophyCategory.RelogioMusical, TrophyRarity.Legendary));

        // CATEGORIA: ESPECIAIS/SECRETOS - TROFÉUS MISTERIOSOS (15 troféus - IDs 86-100)

        // 🎭 EASTER EGGS E SEGREDOS DIVERTIDOS
        trofeus.Add(CreateTrophy("Detetive Musical", "Sherlock Holmes ficaria orgulhoso!", "???", "🕵️", 500, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Encontre a música que não deveria estar lá..."));
        trofeus.Add(CreateTrophy("Noturno", "A música soa diferente na madrugada...", "???", "🌙", 300, TrophyCategory.Especiais, TrophyRarity.Rare, true, "Jogue entre meia-noite e 6h da manhã"));
        trofeus.Add(CreateTrophy("Madrugador", "Pássaro que madriga, música encontra!", "???", "🌅", 300, TrophyCategory.Especiais, TrophyRarity.Rare, true, "Jogue entre 5h e 7h da manhã"));
        trofeus.Add(CreateTrophy("Perseverança", "Nunca desista! A música recompensa a dedicação.", "???", "💪", 600, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Continue jogando mesmo depois de errar 20 vezes seguidas"));
        trofeus.Add(CreateTrophy("Sortudo", "As estrelas se alinharam para você!", "???", "🍀", 777, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Acerte uma música no primeiro segundo por pura sorte"));
        trofeus.Add(CreateTrophy("Perfil Baixo", "Às vezes é melhor não chamar atenção...", "???", "🤫", 400, TrophyCategory.Especiais, TrophyRarity.Rare, true, "Jogue por 30 minutos sem ganhar nenhum outro troféu"));
        trofeus.Add(CreateTrophy("Volta ao Passado", "A nostalgia é uma força poderosa!", "???", "⏪", 500, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Acerte 5 músicas da sua infância em sequência"));
        trofeus.Add(CreateTrophy("Conexão Perdida", "Nem a internet pode parar a música!", "???", "📶", 350, TrophyCategory.Especiais, TrophyRarity.Rare, true, "Continue jogando mesmo com problemas de conexão"));
        trofeus.Add(CreateTrophy("Minimalista", "Menos é mais... até nos troféus!", "???", "⚪", 200, TrophyCategory.Especiais, TrophyRarity.Common, true, "Ganhe apenas troféus Common por uma semana"));
        trofeus.Add(CreateTrophy("Colecionador", "Tem que pegar todos!", "???", "🏆", 1000, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Desbloqueie troféus de todas as categorias no mesmo dia"));
        trofeus.Add(CreateTrophy("Silêncio Musical", "Às vezes o silêncio diz mais que mil notas...", "???", "🔇", 400, TrophyCategory.Especiais, TrophyRarity.Rare, true, "Jogue com o som desligado e mesmo assim acerte 5 músicas"));
        trofeus.Add(CreateTrophy("Déjà Vu", "Será que já vivi isso antes?", "???", "🌀", 600, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Acerte a mesma música 3 vezes no mesmo dia"));
        trofeus.Add(CreateTrophy("Impossível", "O impossível é apenas uma questão de tempo!", "???", "❌", 2000, TrophyCategory.Especiais, TrophyRarity.Legendary, true, "Faça algo que deveria ser impossível..."));
        trofeus.Add(CreateTrophy("Quase Lá", "Tão perto da perfeição... tão longe ao mesmo tempo.", "???", "🎯", 999, TrophyCategory.Especiais, TrophyRarity.Epic, true, "Chegue a 99% de alguma estatística"));
        trofeus.Add(CreateTrophy("Centena Sagrada", "O número 100 tem poderes místicos!", "???", "💯", 1000, TrophyCategory.Especiais, TrophyRarity.Legendary, true, "Este é o troféu #100... isso deve significar algo!"));

        // CATEGORIA: LOJA - TROFÉUS CRIATIVOS (15 troféus - IDs 101-115)

        // 🌟 BÁSICOS (Common - IDs 101-105)
        trofeus.Add(CreateTrophy("Primeira Compra", "Todo grande investidor começou assim!", "Compre seu primeiro item na loja", "🛒", 20, TrophyCategory.Loja, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Cliente Fiel", "A loja já te conhece pelo nome!", "Faça 3 compras na loja", "🤝", 50, TrophyCategory.Loja, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Economista", "Sabe o valor de cada ponto ganho!", "Compre 5 itens gastando menos de 500 pontos total", "💰", 75, TrophyCategory.Loja, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Colecionador Iniciante", "A coleção começou a crescer!", "Compre itens de 3 categorias diferentes", "📦", 100, TrophyCategory.Loja, TrophyRarity.Common));
        trofeus.Add(CreateTrophy("Investidor", "Seus pontos são bem aplicados!", "Compre 10 itens na loja", "📈", 150, TrophyCategory.Loja, TrophyRarity.Common));

        // 🔥 INTERMEDIÁRIOS (Rare - IDs 106-110)
        trofeus.Add(CreateTrophy("Comprador Compulsivo", "Se está na loja, precisa comprar!", "Faça 15 compras na loja", "🛍️", 300, TrophyCategory.Loja, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Magnata", "Dinheiro não é problema para você!", "Gaste mais de 5.000 pontos na loja", "💎", 500, TrophyCategory.Loja, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Caçador de Ofertas", "Sempre encontra as melhores promoções!", "Compre 5 itens em promoção", "🏷️", 400, TrophyCategory.Loja, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Curador", "Tem gosto refinado para escolhas!", "Compre apenas itens Epic ou Legendary", "🎭", 600, TrophyCategory.Loja, TrophyRarity.Rare));
        trofeus.Add(CreateTrophy("Patrono da Loja", "Praticamente sustenta o negócio sozinho!", "Faça compras por 10 dias diferentes", "🏪", 750, TrophyCategory.Loja, TrophyRarity.Rare));

        // ⚡ AVANÇADOS (Epic - IDs 111-115)
        trofeus.Add(CreateTrophy("Imperador Comercial", "Domina o mercado como ninguém!", "Compre 50 itens na loja", "👑", 1000, TrophyCategory.Loja, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Milionário", "Gastou uma fortuna em música!", "Gaste mais de 50.000 pontos na loja total", "💰", 2000, TrophyCategory.Loja, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Colecionador Supremo", "Tem pelo menos um item de cada categoria!", "Compre itens de todas as categorias disponíveis", "🏆", 1500, TrophyCategory.Loja, TrophyRarity.Epic));
        trofeus.Add(CreateTrophy("Viciado em Compras", "A loja é sua segunda casa!", "Faça uma compra por 30 dias consecutivos", "🔄", 3000, TrophyCategory.Loja, TrophyRarity.Legendary));
        trofeus.Add(CreateTrophy("Deus do Consumo", "Transcendeu a necessidade material... mas ainda compra!", "Compre 100 itens na loja", "⚡", 5000, TrophyCategory.Loja, TrophyRarity.Legendary));

        return trofeus;
    }

    private static async Task GetDefaultSongs(ApplicationDbContext context)
    {
        if (!await context.Songs.AnyAsync())
        {
            var songs = new List<Song>
            {
                CreateSong("Boulevard Of Broken Dreams", "Green Day", 2004, SongsGenre.Rock, 25, true, "1"),
                CreateSong("Poker Face", "Lady Gaga", 2008, SongsGenre.Pop, 25, true, "2"),
                CreateSong("Dream on", "Aerosmith", 1973, SongsGenre.Rock, 25, true, "3"),
                CreateSong("Africa", "Toto", 1982, SongsGenre.Rock, 25, true, "4"),
                CreateSong("After Dark", "Tito & Tarantula", 1996, SongsGenre.Rock, 25, true, "5"),
                CreateSong("Another Brick In The Wall", "Pink Floyd", 1979, SongsGenre.Rock, 25, true, "6"),
                CreateSong("Billie Jean", "Michael Jackson", 1982, SongsGenre.Pop, 25, true, "7"),
                CreateSong("redbone", "Childish Gambino", 2016, SongsGenre.RBSoul, 25, true, "8"),
                CreateSong("Clint Eastwood", "Gorillaz", 2001, SongsGenre.HipHopRap, 25, true, "9"),
                CreateSong("Crazy Train", "Ozzy Osbourne", 1980, SongsGenre.Rock, 25, true, "10"),
                CreateSong("Down Under", "Men at Work", 1981, SongsGenre.Rock, 25, true, "11"),
                CreateSong("Dust In The Wind", "Kansas", 1977, SongsGenre.Rock, 25, true, "12"),
                CreateSong("Enter Sandman", "Metallica", 1991, SongsGenre.Rock, 25, true, "13"),
                CreateSong("Hallowed Be Thy Name", "Iron Maiden", 1982, SongsGenre.Rock, 25, true, "14"),
                CreateSong("Havana", "Camila Cabello", 2017, SongsGenre.Pop, 25, true, "15"),
                CreateSong("Immigrant Song", "Led Zeppelin", 1970, SongsGenre.Rock, 25, true, "16"),
                CreateSong("In the End", "Linkin Park", 2000, SongsGenre.Rock, 25, true, "17"),
                CreateSong("Judas", "Lady Gaga", 2011, SongsGenre.Pop, 25, true, "18"),
                CreateSong("Kashmir", "Led Zeppelin", 1975, SongsGenre.Rock, 25, true, "19"),
                CreateSong("Lean On", "Major Lazer & DJ Snake", 2015, SongsGenre.EletronicaEDM, 25, true, "20"),
                CreateSong("Hung Up", "Madonna", 2005, SongsGenre.Pop, 25, true, "21"),
                CreateSong("Nothing Else Matters", "Metallica", 1991, SongsGenre.Rock, 25, true, "22"),
                CreateSong("Numb", "Linkin Park", 2003, SongsGenre.Rock, 25, true, "23"),
                CreateSong("One", "Metallica", 1988, SongsGenre.Rock, 25, true, "24"),
                CreateSong("Apologize", "Timbaland ft. OneRepublic", 2007, SongsGenre.Pop, 25, true, "25"),
                CreateSong("Paint It Black", "The Rolling Stones", 1966, SongsGenre.Rock, 25, true, "26"),
                CreateSong("Paparazzi", "Lady Gaga", 2008, SongsGenre.Pop, 25, true, "27"),
                CreateSong("Psycho Killer", "Talking Heads", 1977, SongsGenre.Rock, 25, true, "28"),
                CreateSong("Royals", "Lorde", 2013, SongsGenre.Pop, 25, true, "29"),
                CreateSong("Run to the Hills", "Iron Maiden", 1982, SongsGenre.Rock, 25, true, "30"),
                CreateSong("Running Up That Hill", "Kate Bush", 1985, SongsGenre.Pop, 25, true, "31"),
                CreateSong("Hips Dont Lie", "Shakira ft. Wyclef Jean", 2006, SongsGenre.Pop, 25, true, "32"),
                CreateSong("Shape of You", "Ed Sheeran", 2017, SongsGenre.Pop, 25, true, "33"),
                CreateSong("Smoke On The Water", "Deep Purple", 1972, SongsGenre.Rock, 25, true, "34"),
                CreateSong("Stairway to Heaven", "Led Zepllin", 1971, SongsGenre.Rock, 25, true, "35"),
                CreateSong("Still D.R.E.", "Dr. Dre ft. Snoop Dogg", 1999, SongsGenre.HipHopRap, 25, true, "36"),
                CreateSong("Sweet Dreams", "Eurythmics", 1983, SongsGenre.Pop, 25, true, "37"),
                CreateSong("Take On Me", "a-ha", 1984, SongsGenre.Pop, 25, true, "38"),
                CreateSong("The Final Countdown", "Europe", 1986, SongsGenre.Rock, 25, true, "39"),
                CreateSong("The Number of the Beast", "Iron Maiden", 1982, SongsGenre.Rock, 25, true, "40"),
                CreateSong("Thunderstruck", "AC/DC", 1990, SongsGenre.Rock, 25, true, "41"),
                CreateSong("Titanium", "David Guetta ft. Sia", 2011, SongsGenre.EletronicaEDM, 25, true, "42"),
                CreateSong("Wait and Bleed", "Slipknot", 1999, SongsGenre.Rock, 25, true, "43"),
                CreateSong("Where Is My Mind", "Pixies", 1988, SongsGenre.Rock, 25, true, "44"),
                CreateSong("Breaking the Habit", "Linkin Park", 2003, SongsGenre.Rock, 25, true, "45"),

                // Novas músicas (46-259) com Is_8Bit = false
                CreateSong("Break Ya Neck", "Busta Rhymes", 2001, SongsGenre.HipHopRap, 25, false, "46"),
                CreateSong("Burning Love", "Elvis Presley", 1972, SongsGenre.Rock, 25, false, "47"),
                CreateSong("Cachimbo da Paz", "Racionais MC's", 1997, SongsGenre.HipHopRap, 25, false, "48"),
                CreateSong("Cheia de Manias", "Raça Negra", 1990, SongsGenre.Sertanejo, 25, false, "49"),
                CreateSong("Clint Eastwood", "Gorillaz", 2001, SongsGenre.HipHopRap, 25, false, "50"),
                CreateSong("Couro de Boi", "Almir Sater", 1997, SongsGenre.Sertanejo, 25, false, "51"),
                CreateSong("Crank That", "Soulja Boy", 2007, SongsGenre.HipHopRap, 25, false, "52"),
                CreateSong("Crazy", "Gnarls Barkley", 2006, SongsGenre.Pop, 25, false, "53"),
                CreateSong("Creep", "Radiohead", 1992, SongsGenre.Rock, 25, false, "54"),
                CreateSong("De igual pra igual", "Racionais MC's", 2006, SongsGenre.HipHopRap, 25, false, "55"),
                CreateSong("Deus Me Proteja", "Catedral", 1995, SongsGenre.Rock, 25, false, "56"),
                CreateSong("Dilemma", "Nelly ft. Kelly Rowland", 2002, SongsGenre.HipHopRap, 25, false, "57"),
                CreateSong("Dirt Off Your Shoulder", "Jay-Z", 2003, SongsGenre.HipHopRap, 25, false, "58"),
                CreateSong("Dormi na praça", "Bruno & Marrone", 1998, SongsGenre.Sertanejo, 25, false, "59"),
                CreateSong("Drop It Like It's Hot", "Snoop Dogg ft. Pharrell", 2004, SongsGenre.HipHopRap, 25, false, "60"),
                CreateSong("Empire State Of Mind", "Jay-Z ft. Alicia Keys", 2009, SongsGenre.HipHopRap, 25, false, "61"),
                CreateSong("Equalize", "Pitty", 2005, SongsGenre.Rock, 25, false, "62"),
                CreateSong("Estou apaixonado", "Exaltasamba", 2001, SongsGenre.Pagode, 25, false, "63"),
                CreateSong("Even Flow", "Pearl Jam", 1991, SongsGenre.Rock, 25, false, "64"),
                CreateSong("Every Breath You Take", "The Police", 1983, SongsGenre.Rock, 25, false, "65"),
                CreateSong("Facas", "Diego & Victor Hugo", 2020, SongsGenre.Sertanejo, 25, false, "66"),
                CreateSong("Fortunate Son", "Creedence Clearwater Revival", 1969, SongsGenre.Rock, 25, false, "67"),
                CreateSong("Garota Nacional", "Skank", 1993, SongsGenre.Rock, 25, false, "68"),
                CreateSong("Get Buck", "Young Buck", 2004, SongsGenre.HipHopRap, 25, false, "69"),
                CreateSong("Get No Better", "CID", 2017, SongsGenre.EletronicaEDM, 25, false, "70"),
                CreateSong("Gold Digger", "Kanye West ft. Jamie Foxx", 2005, SongsGenre.HipHopRap, 25, false, "71"),
                CreateSong("Hey Joe", "The Jimi Hendrix Experience", 1966, SongsGenre.Rock, 25, false, "72"),
                CreateSong("Hey Ya", "OutKast", 2003, SongsGenre.HipHopRap, 25, false, "73"),
                CreateSong("Hot In Herre", "Nelly", 2002, SongsGenre.HipHopRap, 25, false, "74"),
                CreateSong("How We Do", "The Game ft. 50 Cent", 2004, SongsGenre.HipHopRap, 25, false, "75"),
                CreateSong("Hustlin'", "Rick Ross", 2006, SongsGenre.HipHopRap, 25, false, "76"),
                CreateSong("I Know What You Want", "Busta Rhymes ft. Mariah Carey", 2003, SongsGenre.HipHopRap, 25, false, "77"),
                CreateSong("I Wanna Be Sedated", "Ramones", 1978, SongsGenre.Rock, 25, false, "78"),
                CreateSong("If I Ruled the World", "Nas ft. Lauryn Hill", 1996, SongsGenre.HipHopRap, 25, false, "79"),
                CreateSong("I'm a Believer", "The Monkees", 1966, SongsGenre.Pop, 25, false, "80"),
                CreateSong("In Da Club", "50 Cent", 2003, SongsGenre.HipHopRap, 25, false, "81"),
                CreateSong("Interlude", "Attack Attack!", 2008, SongsGenre.Rock, 25, false, "82"),
                CreateSong("Jeito Sexy", "Tchakabum", 1997, SongsGenre.ForroAxe, 25, false, "83"),
                CreateSong("Just A Lil Bit", "50 Cent", 2005, SongsGenre.HipHopRap, 25, false, "84"),
                CreateSong("Kiss Kiss", "Chris Brown ft. T-Pain", 2007, SongsGenre.RBSoul, 25, false, "85"),
                CreateSong("Kiss Me Thru The Phone", "Soulja Boy ft. Sammie", 2008, SongsGenre.HipHopRap, 25, false, "86"),
                CreateSong("Knockin' On Heavens Door", "Bob Dylan", 1973, SongsGenre.Rock, 25, false, "87"),
                CreateSong("La Bamba", "Ritchie Valens", 1958, SongsGenre.Rock, 25, false, "88"),
                CreateSong("Lean Back", "Terror Squad", 2004, SongsGenre.HipHopRap, 25, false, "89"),
                CreateSong("Let Me Blow Ya Mind", "Eve ft. Gwen Stefani", 2001, SongsGenre.HipHopRap, 25, false, "90"),
                CreateSong("Let Me Love You", "Mario", 2004, SongsGenre.RBSoul, 25, false, "91"),
                CreateSong("Liguei Pra Dizer Que Te Amo", "RPM", 1985, SongsGenre.Rock, 25, false, "92"),
                CreateSong("Like a Stone", "Audioslave", 2002, SongsGenre.Rock, 25, false, "93"),
                CreateSong("Locked Up", "Akon", 2004, SongsGenre.RBSoul, 25, false, "94"),
                CreateSong("Lollipop", "Lil Wayne ft. Static Major", 2008, SongsGenre.HipHopRap, 25, false, "95"),
                CreateSong("Lonely", "Akon", 2005, SongsGenre.RBSoul, 25, false, "96"),
                CreateSong("Lonely Day", "System of a Down", 2005, SongsGenre.Rock, 25, false, "97"),
                CreateSong("Long Way 2 Go", "Cassie", 2006, SongsGenre.RBSoul, 25, false, "98"),
                CreateSong("Lose Yourself", "Eminem", 2002, SongsGenre.HipHopRap, 25, false, "99"),
                CreateSong("Low", "Flo Rida ft. T-Pain", 2007, SongsGenre.HipHopRap, 25, false, "100"),
                CreateSong("Mamae Passou Acucar Em Mim", "Mamonas Assassinas", 1995, SongsGenre.Rock, 25, false, "259"),
            };

            await context.Songs.AddRangeAsync(songs);
            await context.SaveChangesAsync();
        }
    }

    private static async Task GetDefaultQuizQuestions(ApplicationDbContext context)
    {
        if (!context.QuizQuestions.Any())
        {

            var questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o bairro que o João e em breve você irá morar?🏦 Ex: Mussumagro",
                    Answer = "Bessa",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o nome do marido da Charlotte (Iscarleti Iorransão) no filme Lost in Translation? 🎥 Ex: jurema",
                    Answer = "John",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o nome do nosso prédio em Joao Pessoa? Residencial ... (somente o segundo nome) 🏦  Ex: Priquito",
                    Answer = "Arpoador",
                    Points = 10,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quantos corações você utilizou quando usou o tradicional coração vermelho e preto para despedir toda noite?❤️🖤 Ex:9",
                    Answer = "5",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o nome dos meus (e futuro seus também) dois cachorros? 🐶🐶  Ex: Sandy Junior",
                    Answer = "thor gandalf",
                    Points = 10,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o nome do evento no centro de João Pessoa que planejamos ir 💃? Ex: Cabare de cego",
                    Answer = "sabadinho bom",
                    Points = 10,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o placar do primeiro jogo que vimos juntos⚽🏈? Ex: 1x0",
                    Answer = "3x0",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Que dia você comprou o BALAAAATRO? 🃏🃏 Ex: 28/04/1993",
                    Answer = "06/06/2025",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual o ano da camisa que você comprou para mim de dia dos namorado💞? Ex: 1996",
                    Answer = "2024",
                    Points = 10,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual a chave pix que você utilizou para me mandar meus 20tim💸🤑? Ex: 2345678",
                    Answer = "05135884181",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion{
                    Id = Guid.NewGuid(),
                    Question = "Qual banda britânica de rock é responsável pela música 'Another Brick In The Wall'? 🎸",
                    Answer = "Pink Floyd",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Poker Face', uma das músicas mais famosas do pop eletrônico? 🎤",
                    Answer = "Lady Gaga",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual é o nome do artista que ficou famoso com 'Crazy Train'? 🤘",
                    Answer = "Ozzy Osbourne",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De qual banda é a música 'Enter Sandman', um hino do heavy metal? ⚡",
                    Answer = "Metallica",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Havana', sucesso que mistura pop e ritmos latinos? 🎶",
                    Answer = "Camila Cabello",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock liderada por Chester Bennington canta 'In the End'? 🔥",
                    Answer = "Linkin Park",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem é o artista por trás de 'redbone', música que viralizou nas redes sociais? 🎵",
                    Answer = "Childish Gambino",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda virtual criada por Damon Albarn canta 'Clint Eastwood'? 🎭",
                    Answer = "Gorillaz",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a voz em 'Billie Jean', um dos maiores sucessos da história do pop? 👑",
                    Answer = "Michael Jackson",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock australiana canta 'Down Under'? 🦘",
                    Answer = "Men at Work",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Sweet Dreams', hit dos anos 80 com sintetizadores marcantes? 💤",
                    Answer = "Eurythmics",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda norueguesa é responsável por 'Take On Me'? 🎹",
                    Answer = "a-ha",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Africa', regravada por diversos artistas? 🌍",
                    Answer = "Toto",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual rapper fez sucesso com 'Lose Yourself', trilha sonora de 8 Mile? 🎬",
                    Answer = "Eminem",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Shape of You', um dos maiores sucessos da década de 2010? 🔺",
                    Answer = "Ed Sheeran",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock britânica canta 'Smoke On The Water'? 💨",
                    Answer = "Deep Purple",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Still D.R.E.', featuring com Snoop Dogg? 🎧",
                    Answer = "Dr. Dre",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual cantora pop canta 'Hung Up', sampleando música do ABBA? ⏰",
                    Answer = "Madonna",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Titanium', em parceria com David Guetta? 💎",
                    Answer = "Sia",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock canta 'Sweet Child O' Mine'? 🌹",
                    Answer = "Guns N' Roses",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Wonderwall', um hino dos anos 90? 🧱",
                    Answer = "Oasis",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual rapper canta 'Gold Digger' em parceria com Jamie Foxx? 💰",
                    Answer = "Kanye West",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'La Bamba', música tradicional mexicana que virou hit rock? 💃",
                    Answer = "Ritchie Valens",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock alternativo canta 'Creep'? 🐾",
                    Answer = "Radiohead",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Every Breath You Take', da banda The Police? 👮",
                    Answer = "Sting",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual grupo de rap brasileiro é responsável por 'Cachimbo da Paz'? ☮️",
                    Answer = "Racionais MC's",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Garota Nacional', hit do rock brasileiro dos anos 90? 🇧🇷",
                    Answer = "Skank",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual dupla sertaneja canta 'Dormi na praça'? 🌙",
                    Answer = "Bruno & Marrone",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Malemolência', também conhecida como 'Malandragem'? 😎",
                    Answer = "Cássia Eller",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual grupo de pagode canta 'Estou apaixonado'? 💘",
                    Answer = "Exaltasamba",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Pense em mim', sucesso sertanejo dos anos 90? 💭",
                    Answer = "Leandro & Leonardo",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda brasileira de rock canta 'Pelados Em Santos'? 🏖️",
                    Answer = "Mamonas Assassinas",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Liguei Pra Dizer Que Te Amo'? 📞",
                    Answer = "RPM",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual cantora brasileira canta 'Porta aberta'? 🚪",
                    Answer = "Rita Lee",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Quero Te Encontrar', sucesso do samba-pop? 🔍",
                    Answer = "Claudinho & Buchecha",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual grupo de axé canta 'Ralando O Tchan'? 💃",
                    Answer = "É o Tchan!",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Sonhei Com Voce', do Jota Quest? 💤",
                    Answer = "Jota Quest",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual rapper canta 'Stan', em parceria com Dido? ✉️",
                    Answer = "Eminem",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Vai Dar Namoro', hit sertanejo romântico? 💑",
                    Answer = "Leandro & Leonardo",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock brasileiro canta 'Tédio'? 😴",
                    Answer = "Biquini Cavadao",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Robocop Gay', dos Mamonas Assassinas? 🤖",
                    Answer = "Mamonas Assassinas",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual cantor brega canta 'Eu Não Sou Cachorro Não'? 🐶",
                    Answer = "Waldick Soriano",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'Chalana', música caipira tradicional? 🎻",
                    Answer = "Mário Zan",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda brasileira canta 'Admirável Chip Novo'? 🔌",
                    Answer = "Pitty",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'Coração Vagabundo', da Tropicália? 🌴",
                    Answer = "Caetano Veloso",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual grupo musical canta 'Ragatanga'? 🎉",
                    Answer = "Rouge",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Quem canta 'It Must Have Been Love', trilha de Pretty Woman? 💔",
                    Answer = "Roxette",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual cantora de jazz canta 'Smooth Operator'? 🎷",
                    Answer = "Sade",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "De quem é a música 'I Want To Hold Your Hand', dos Beatles? 🤝",
                    Answer = "The Beatles",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                },
                new QuizQuestion
                {
                    Id = Guid.NewGuid(),
                    Question = "Qual banda de rock psicodélico canta 'Riders on the Storm'? 🌧️",
                    Answer = "The Doors",
                    Points = 5,
                    IsAnswered = false,
                    CreatedAt = DateTime.UtcNow,
                }

            };
            context.QuizQuestions.AddRange(questions);
            context.SaveChanges();
        }
    }
    private static Song CreateSong(string name, string artist, int year, SongsGenre genre, int points, bool is8bit, string fileId)
    {
        return new Song
        {
            Id = Guid.NewGuid(),
            Name = name,
            Artist = artist,
            Year = year,
            Genre = genre,
            Points = points,
            Is8Bit = is8bit,
            FileId = fileId
        };
    }
    // Método Helper para criar a Entidade e gerar o Slug automaticamente
    private static Trophy CreateTrophy(string name, string description, string requirements, string icon, int points, TrophyCategory category, TrophyRarity rarity, bool isSecret = false, string? secretHint = null)
    {
        string slugCombinado = GenerateSlug($"{category.ToString()}_{name}");
        return new Trophy
        {
            Id = Guid.NewGuid(),
            Slug = slugCombinado,
            Name = name,
            Description = description,
            Requirements = requirements,
            Icon = icon,
            Points = points,
            Category = category,
            Rarity = rarity,
            IsSecret = isSecret,
            SecretHint = secretHint
        };
    }

    // Converte textos para Slugs (Maiúsculo, sem acentos, sem espaços)
    private static string GenerateSlug(string text)
    {
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        var cleanString = stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC).ToUpper();
        cleanString = Regex.Replace(cleanString, @"[^A-Z0-9\s-]", "");
        cleanString = Regex.Replace(cleanString, @"[\s-]+", "_").Trim('_');

        return cleanString;
    }
}