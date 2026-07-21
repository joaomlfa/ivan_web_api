using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Interfaces
{
    public interface ITrophyRule
    {
        List<string> Evaluate(PlayerProfile profile, object eventContext);
    }
}