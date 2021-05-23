using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.Fight
{
    public class WeaponAttackResultDto
    {
        public string AttackerName { get; set; }
        public int AttackerHP { get; set; }
        public string OpponentName { get; set; }
        public int OpponentHP { get; set; }
        public int Damage { get; set; }
    }
}
