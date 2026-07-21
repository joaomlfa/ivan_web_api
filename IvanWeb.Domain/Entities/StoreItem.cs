namespace IvanWeb.Domain.Entities;

public class StoreItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Icon { get; set; } = string.Empty; // Ex: "🍫"

    // Se for true, o item some da loja depois que ela comprar
    public bool IsSinglePurchase { get; set; }

    // Se for true, a API vai devolver o link do WhatsApp no momento da compra
    public bool SendWhatsApp { get; set; }

    public bool IsActive { get; set; } = true; // Para você poder "esconder" itens da loja no futuro sem deletar do banco
}