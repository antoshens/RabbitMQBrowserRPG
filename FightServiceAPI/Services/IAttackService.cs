using FightServiceAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FightServiceAPI.Services
{
    public interface IAttackService
    {
        Task<List<AttackLog>> GetAllAttackLogs();
        Task AddAttackLogResponse(AttackLog attackLog);
        Task AddAttackLog(AttackLog attackLog);
    }
}
