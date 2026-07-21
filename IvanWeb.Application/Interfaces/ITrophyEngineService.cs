using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvanWeb.Domain.Entities;

namespace IvanWeb.Application.Interfaces
{
    public interface ITrophyEngineService
    {
        Task<List<Trophy>> ProcessTrophiesAsync(Guid playerId, object eventContext);
    }
}