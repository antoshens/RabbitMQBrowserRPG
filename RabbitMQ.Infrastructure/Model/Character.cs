using BrowserTextRPG.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Model
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = 0;
        [Required]
        public string Name { get; set; }

        public int Health { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public CharacterTypeEnum charType { get; set; }
        public User User { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public Weapon Weapon { get; set; }
        public List<CharacterSkill> CharacterSkills { get; set; }

        // Not administratable fields
        public DateTime? CreationDateTime { get; set; } = default;
        public DateTime? ModificationDateTime {get; set;} = default;

        public int Fights { get; set; } = 0;
        public int Defeats { get; set; } = 0;
        public int Victories { get; set; } = 0;
    }
}
