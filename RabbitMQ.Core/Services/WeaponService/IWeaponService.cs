using BrowserTextRPG.DTOModel.Weapon;
using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<GatewayResponse<GetWeaponDto>> Add(AddWeaponDto weapon);
        Task<GatewayResponse<bool>> Delete(DeleteWeaponDto weapon);
    }
}
