using AutoMapper;
using BrowserTextRPG.Data;
using BrowserTextRPG.DTOModel.Character;
using BrowserTextRPG.DTOModel.Fight;
using BrowserTextRPG.DTOModel.Skill;
using BrowserTextRPG.DTOModel.User;
using BrowserTextRPG.DTOModel.Weapon;
using BrowserTextRPG.Model;
using System.Linq;

namespace BrowserTextRPG
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ========== Character mappers ==========
            // FROM Character TO CharacterDtos
            CreateMap<Character, GetCharacterDto>()
                .ForMember(dto => dto.Skills,
                c => c.MapFrom(c => c.CharacterSkills.Select(cs => cs.Skill))); // Needed for many-to-many relation mapping
            CreateMap<Character, AddCharacterDto>();
            CreateMap<Character, ModifyCharacterDto>();
            CreateMap<Character, DeleteCharacterDto>();
            CreateMap<Character, GetHighscoreDto>();

            // FROM CharacterDtos TO Character
            CreateMap<GetCharacterDto, Character>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<ModifyCharacterDto, Character>();
            CreateMap<DeleteCharacterDto, Character>();

            // ========== User mappers ==========
            CreateMap<GetUserDto, User>();

            CreateMap<User, GetUserDto>();

            // ========== Weapon mappers ==========
            // FROM Weapon TO WeaponDtos
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Weapon, AddWeaponDto>();
            CreateMap<Weapon, DeleteWeaponDto>();

            // FROM WeaponDtos TO Weapon
            CreateMap<GetWeaponDto, Weapon>();
            CreateMap<AddWeaponDto, Weapon>();
            CreateMap<DeleteWeaponDto, Weapon>();

            // ========== Skill mappers ==========
            CreateMap<Skill, GetSkillDto>();

            CreateMap<GetSkillDto, Skill>();
        }
    }
}
