using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace IvanWeb.Domain.Entities;

public class PurchaseHistory : EntityBase
{
    public Guid PlayerProfileId { get; set; }
    public Guid StoreItemId { get; set; }

    // Propriedades de Navegação
    public PlayerProfile? PlayerProfile { get; set; }
    public StoreItem? StoreItem { get; set; }
}