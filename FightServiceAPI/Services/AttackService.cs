using FightServiceAPI.Data;
using FightServiceAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FightServiceAPI.Services
{
    public class AttackService : IAttackService
    {
        private readonly AttackDataContext _dbContext;

        public AttackService(AttackDataContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<AttackLog>> GetAllAttackLogs()
        {
           return await this._dbContext.AttackLogs.ToListAsync();
        }

        public async Task AddAttackLog(AttackLog attackLog)
        {
            this._dbContext.AttackLogs.Add(attackLog);

            await this._dbContext.SaveChangesAsync();
        }
    }
}
