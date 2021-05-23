using BrowserTextRPG.DTOModel.Fight;
using BrowserTextRPG.Model;
using BrowserTextRPG.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            this._fightService = fightService;
        }

        [HttpPost("Weapon")]
        public async Task<ActionResult<GatewayResponse<WeaponAttackResultDto>>> WeaponAttack([FromBody]WeaponAttackDto weaponAttack)
        {
            var response = await this._fightService.WeaponAttack(weaponAttack);

            return Ok(response);
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<GatewayResponse<SkillAttackResultDto>>> SkillAttack([FromBody]SkillAttackDto skillAttack)
        {
            var response = await this._fightService.SkillAttack(skillAttack);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GatewayResponse<FightResponseDto>>> AutoFight([FromBody]FightRequestDto fightRequest)
        {
            var response = await this._fightService.AutoFight(fightRequest);

            return Ok(response);
        }

        [HttpGet("Highscore")]
        [AllowAnonymous]
        public async Task<ActionResult<GatewayResponse<List<GetHighscoreDto>>>> GetHighscore()
        {
            var response = await this._fightService.GetHighscore();

            return Ok(response);
        }
    }
}
