using BrowserTextRPG.DTOModel.Fight;
using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.FightService
{
    public interface IFightService
    {
        Task<GatewayResponse<WeaponAttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
        Task<GatewayResponse<SkillAttackResultDto>> SkillAttack(SkillAttackDto skillAttack);
        Task<GatewayResponse<FightResponseDto>> AutoFight(FightRequestDto fightRequest);
        Task<GatewayResponse<List<GetHighscoreDto>>> GetHighscore();
    }
}
