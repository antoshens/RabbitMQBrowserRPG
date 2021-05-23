using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.Weapon
{
    public class GetWeaponDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
    }
}
