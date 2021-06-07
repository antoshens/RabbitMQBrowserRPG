
namespace FightServiceAPI.Commands
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
