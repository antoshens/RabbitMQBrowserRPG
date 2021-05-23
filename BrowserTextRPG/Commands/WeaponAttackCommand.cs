using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Commands
{
    public class WeaponAttackCommand : AttackCommand
    {
        public WeaponAttackCommand(int attacker, int opponent, int damage)
        {
            AttackerId = attacker;
            OpponentId = opponent;
            Damage = damage;
        }
    }
}
