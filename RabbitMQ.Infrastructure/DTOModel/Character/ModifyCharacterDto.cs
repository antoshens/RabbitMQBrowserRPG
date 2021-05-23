using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.Character
{
    public class ModifyCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Health { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public CharacterTypeEnum CharType { get; set; }
    }
}
