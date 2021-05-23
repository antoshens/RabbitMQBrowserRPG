using BrowserTextRPG.DTOModel.Weapon;
using BrowserTextRPG.Model;
using BrowserTextRPG.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BrowserTextRPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            this._weaponService = weaponService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<GatewayResponse<GetWeaponDto>>> Add(AddWeaponDto weapon)
        {
            var response = await this._weaponService.Add(weapon);

            return Ok(response);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<GatewayResponse<bool>>> Delete(DeleteWeaponDto weapon)
        {
            var response = await this._weaponService.Delete(weapon);

            return Ok(response);
        }
    }
}
