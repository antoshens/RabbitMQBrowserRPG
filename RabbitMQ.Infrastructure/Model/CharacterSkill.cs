using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Model
{
    public class CharacterSkill
    {
        [ForeignKey("Character")]
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        public Skill Skill { get;  set; }
    }
}
