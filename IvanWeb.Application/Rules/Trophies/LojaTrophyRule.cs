using IvanWeb.Application.Interfaces;
using IvanWeb.Application.Models.Events;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Rules;

public class LojaTrophyRule : ITrophyRule
{
    public List<string> Evaluate(PlayerProfile player, object eventContext)
    {
        var unlockedSlugs = new List<string>();

        // Verifica se o evento disparado é realmente uma compra na loja
        if (eventContext is not StorePurchaseEvent storeEvent)
            return unlockedSlugs;

        // ==========================================
        // REGRAS DE QUANTIDADE DE COMPRAS
        // ==========================================
        if (storeEvent.TotalItemsPurchased >= 1)
            unlockedSlugs.Add("LOJAPRIMEIRA_COMPRA"); // ID 101

        if (storeEvent.TotalItemsPurchased >= 3)
            unlockedSlugs.Add("LOJACLIENTE_FIEL"); // ID 102

        if (storeEvent.TotalItemsPurchased >= 10)
            unlockedSlugs.Add("LOJAINVESTIDOR"); // ID 105

        if (storeEvent.TotalItemsPurchased >= 15)
            unlockedSlugs.Add("LOJACOMPRADOR_COMPULSIVO"); // ID 106

        if (storeEvent.TotalItemsPurchased >= 50)
            unlockedSlugs.Add("LOJAIMPERADOR_COMERCIAL"); // ID 111

        if (storeEvent.TotalItemsPurchased >= 100)
            unlockedSlugs.Add("LOJADEUS_DO_CONSUMO"); // ID 115

        // ==========================================
        // REGRAS DE PONTOS GASTOS
        // ==========================================
        if (storeEvent.TotalPointsSpent >= 5000)
            unlockedSlugs.Add("LOJAMAGNATA"); // ID 107

        if (storeEvent.TotalPointsSpent >= 50000)
            unlockedSlugs.Add("LOJAMILIONARIO"); // ID 112

        // Notas: Troféus de "Dias Consecutivos", "Promoção" ou "Categorias"
        // podem ser adicionados no futuro expandindo a entidade StoreItem e PlayerPurchase!

        return unlockedSlugs;
    }
}