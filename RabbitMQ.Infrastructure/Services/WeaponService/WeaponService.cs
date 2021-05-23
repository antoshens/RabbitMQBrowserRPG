using AutoMapper;
using BrowserTextRPG.Data;
using BrowserTextRPG.DTOModel.Weapon;
using BrowserTextRPG.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WeaponService(DataContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this._dbContext = dbContext;
            this._httpContextAccessor = httpContextAccessor;
            this._mapper = mapper;
        }

        private int GetUserID() => int.Parse(this._httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        public async Task<GatewayResponse<GetWeaponDto>> Add(AddWeaponDto weapon)
        {
            var response = new GatewayResponse<GetWeaponDto>();

            var character = await this._dbContext.Characters.FirstOrDefaultAsync(ch =>
                ch.Id == weapon.CharacterId && ch.User.Id == GetUserID());

            if (character == null)
            {
                response.Fault = new Fault()
                {
                    ErrorCode = 1007,
                    ErrorMessage = "Wrong character Id"
                };

                return response;
            }

            var weaponToAdd = _mapper.Map<AddWeaponDto, Weapon>(weapon);
            weaponToAdd.Character = character;

            // Create a weapon
            this._dbContext.Weapons.Add(weaponToAdd);
            await this._dbContext.SaveChangesAsync();

            response.Data = _mapper.Map<Weapon, GetWeaponDto>(weaponToAdd);

            return response;
        }

        public async Task<GatewayResponse<bool>> Delete(DeleteWeaponDto weapon)
        {
            var response = new GatewayResponse<bool>();

            var character = await this._dbContext.Characters.FirstOrDefaultAsync(ch =>
                ch.Id == weapon.CharacterId && ch.User.Id == GetUserID());

            if (character == null)
            {
                response.Fault = new Fault()
                {
                    ErrorCode = 1007,
                    ErrorMessage = "Wrong character Id"
                };

                return response;
            }

            var weaponToRemove = _mapper.Map<DeleteWeaponDto, Weapon>(weapon);
            weaponToRemove.Character = character;

            // Delete a weapon
            this._dbContext.Weapons.Remove(weaponToRemove);
            await this._dbContext.SaveChangesAsync();

            response.Data = true;

            return response;
        }
    }
}
