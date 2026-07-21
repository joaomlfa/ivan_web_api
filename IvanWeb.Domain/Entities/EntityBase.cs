using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvanWeb.Domain.Entities
{
    public class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}