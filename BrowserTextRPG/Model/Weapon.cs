using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Model
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        [ForeignKey("Character")]
        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
