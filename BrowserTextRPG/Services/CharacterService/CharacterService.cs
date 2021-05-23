using AutoMapper;
using BrowserTextRPG.Data;
using BrowserTextRPG.DTOModel.Character;
using BrowserTextRPG.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public CharacterService(
            IMapper mapper,
            DataContext dbContext,
            IHttpContextAccessor httpContextAcessor)
        {
            this._mapper = mapper;
            this._dbContext = dbContext;
            this._httpContextAcessor = httpContextAcessor;
        }

        private int GetUserID() => int.Parse(this._httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        private string GetRoleName() => this._httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

        public async Task<GatewayResponse<GetCharacterDto>> Add(AddCharacterDto newCharacter)
        {
            var charactedToAdd = this._mapper.Map<AddCharacterDto, Character>(newCharacter);
            GatewayResponse<GetCharacterDto> response = new GatewayResponse<GetCharacterDto>();

            var currentUser = await this._dbContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserID());

            charactedToAdd.User = currentUser;
            charactedToAdd.CreationDateTime = DateTime.Now;

            this._dbContext.Add(charactedToAdd);

            await this._dbContext.SaveChangesAsync();

            response.Data = this._mapper.Map<Character, GetCharacterDto>(charactedToAdd);

            return response;
        }

        public async Task<GatewayResponse<GetCharacterDto>> Modify(ModifyCharacterDto updatedCharacter)
        {
            GatewayResponse<GetCharacterDto> response = new GatewayResponse<GetCharacterDto>();

            if (updatedCharacter.Id == null || updatedCharacter.Id < 1)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1004,
                    ErrorMessage = "Incorrect or missing 'Id' property"
                };

                return response;
            }

            var userCharToModify = await this._dbContext.Characters.Include(ch => ch.Weapon).FirstOrDefaultAsync(ch =>
                ch.Id == updatedCharacter.Id && ch.User != null && ch.User.Id == GetUserID());

            if (userCharToModify == null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1004,
                    ErrorMessage = "Incorrect or missing 'Id' property"
                };

                return response;
            }

            userCharToModify.Name = updatedCharacter.Name;
            userCharToModify.Health = updatedCharacter.Health;
            userCharToModify.Strength = updatedCharacter.Strength;
            userCharToModify.Intelligence = updatedCharacter.Intelligence;
            userCharToModify.Agility = updatedCharacter.Agility;
            userCharToModify.charType = updatedCharacter.CharType;
            userCharToModify.CreationDateTime = userCharToModify.CreationDateTime;
            userCharToModify.ModificationDateTime = DateTime.Now;

            this._dbContext.Characters.Update(userCharToModify);
            await this._dbContext.SaveChangesAsync();

            response.Data = this._mapper.Map<Character, GetCharacterDto>(userCharToModify);

            return response;
        }

        public async Task<GatewayResponse<List<GetCharacterDto>>> Delete(DeleteCharacterDto deletedCharacter)
        {
            GatewayResponse<List<GetCharacterDto>> response = new GatewayResponse<List<GetCharacterDto>>();

            if (deletedCharacter.Id == null || deletedCharacter.Id < 1)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1004,
                    ErrorMessage = "Incorrect or missing 'Id' property"
                };

                return response;
            }

            var userCharToDelete = await this._dbContext.Characters.FirstOrDefaultAsync(ch =>
                ch.Id == deletedCharacter.Id && ch.User != null && ch.User.Id == GetUserID());

            if (userCharToDelete == null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1004,
                    ErrorMessage = "Incorrect or missing 'Id' property"
                };

                return response;
            }

            this._dbContext.Characters.Remove(userCharToDelete);

            await this._dbContext.SaveChangesAsync();

            var userCharacters = await this._dbContext.Characters.Where(ch => ch.User.Id == GetUserID()).ToListAsync();
            response.Data = userCharacters.Select(ch => this._mapper.Map<Character, GetCharacterDto>(ch)).ToList();

            return response;
        }

        public async Task<GatewayResponse<List<GetCharacterDto>>> GetAll()
        {
            GatewayResponse<List<GetCharacterDto>> response = new GatewayResponse<List<GetCharacterDto>>();

            var userCharacters = GetRoleName().Equals("Admin")
                ? await this._dbContext.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill).ToListAsync()
                : await this._dbContext.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                .Where(c => c.User.Id == GetUserID()).ToListAsync();

            response.Data = userCharacters.Select(ch => this._mapper.Map<Character, GetCharacterDto>(ch)).ToList();

            return response;
        }

        public async Task<GatewayResponse<GetCharacterDto>> GetByID(int id)
        {
            GatewayResponse<GetCharacterDto> response = new GatewayResponse<GetCharacterDto>();

            if (id == null || id < 1)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1004,
                    ErrorMessage = "Incorrect or missing 'Id' property"
                };

                return response;
            }

            var dbCharacter = await this._dbContext.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(ch => ch.Id == id && ch.User.Id == GetUserID());
            response.Data = this._mapper.Map<Character, GetCharacterDto>(dbCharacter);

            return response;
        }

        public async Task<GatewayResponse<GetCharacterDto>> AddSkill(AddCharacterSkillDto characterSkill)
        {
            GatewayResponse<GetCharacterDto> response = new GatewayResponse<GetCharacterDto>();

            var character = await this._dbContext.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(ch => ch.Id == characterSkill.CharacterId && ch.User.Id == GetUserID());

            if (character == null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1007,
                    ErrorMessage = "Character has not been found"
                };

                return response;
            }

            var skill = await this._dbContext.Skills.FirstOrDefaultAsync(s => s.Id == characterSkill.SkillId);

            if (skill == null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1008,
                    ErrorMessage = "Skill has not been found"
                };

                return response;
            }

            this._dbContext.CharacterSkills.Add(new CharacterSkill()
            {
                Character = character,
                Skill = skill
            });

            await this._dbContext.SaveChangesAsync();

            response.Data = this._mapper.Map<Character, GetCharacterDto>(character);

            return response;
        }
    }
}
