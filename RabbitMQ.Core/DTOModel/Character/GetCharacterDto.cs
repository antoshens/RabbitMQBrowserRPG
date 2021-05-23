using BrowserTextRPG.Data;
using BrowserTextRPG.DTOModel.Skill;
using BrowserTextRPG.DTOModel.Weapon;
using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public CharacterTypeEnum CharType { get; set; }
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights { get; set; } = 0;
        public int Defeats { get; set; } = 0;
        public int Victories { get; set; } = 0;
    }
}
