
namespace BrowserTextRPG.Commands
{
    public class SkillAttackCommand : AttackCommand
    {
        public SkillAttackCommand(int attacker, int opponent, int damage)
        {
            AttackerId = attacker;
            OpponentId = opponent;
            Damage = damage;
        }
    }
}
