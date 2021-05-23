using FightServiceAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace FightServiceAPI.Data
{
    public class AttackDataContext : DbContext
    {
        public AttackDataContext(DbContextOptions<AttackDataContext> options) : base(options)
        {
        }

        public DbSet<AttackLog> AttackLogs { get; set; }
    }
}
