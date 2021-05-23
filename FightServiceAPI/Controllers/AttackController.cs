using System.Collections.Generic;
using System.Threading.Tasks;
using FightServiceAPI.Model;
using FightServiceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FightServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttackController : ControllerBase
    {
        private IAttackService _attackService;

        public AttackController(IAttackService attackService)
        {
            this._attackService = attackService;
        }

        [HttpGet("attackLogs")]
        public async Task<ActionResult<List<AttackLog>>> GetAll()
        {
            return await this._attackService.GetAllAttackLogs();
        }
    }
}
