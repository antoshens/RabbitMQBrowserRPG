using FightServiceAPI.Commands;
using FightServiceAPI.Data;
using FightServiceAPI.Model;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Core.Bus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FightServiceAPI.Services
{
    public class AttackService : IAttackService
    {
        private readonly AttackDataContext _dbContext;
        private readonly IEventBus _bus;

        public AttackService(AttackDataContext dbContext,
            IEventBus bus)
        {
            this._dbContext = dbContext;
            this._bus = bus;
        }

        public async Task<List<AttackLog>> GetAllAttackLogs()
        {
           return await this._dbContext.AttackLogs.ToListAsync();
        }

        public async Task AddAttackLog(AttackLog attackLog)
        {
            try
            {
                this._dbContext.AttackLogs.Add(attackLog);

                await this._dbContext.SaveChangesAsync();

                var attackFinishedResponse = new AttackResponseCommand(
                    true,
                    "Attack has been finished");

                await this._bus.SendCommand<AttackResponseCommand>(attackFinishedResponse);
            }
            catch(Exception ex)
            {
                var attackFinishedResponse = new AttackResponseCommand(
                    false,
                    "Attack has been failed");

                await this._bus.SendCommand<AttackResponseCommand>(attackFinishedResponse);
            }
        }
    }
}
