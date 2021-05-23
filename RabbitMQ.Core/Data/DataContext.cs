using BrowserTextRPG.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many relation mapping
            modelBuilder.Entity<CharacterSkill>()
                .HasKey(cs => new { cs.CharacterId, cs.SkillId });
            modelBuilder.Entity<CharacterSkill>()
                .HasOne(cs => cs.Character)
                .WithMany(ch => ch.CharacterSkills)
                .HasForeignKey(cs => cs.CharacterId);
            modelBuilder.Entity<CharacterSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany(s => s.CharacterSkills)
                .HasForeignKey(cs => cs.SkillId);

            // Seeding Characters
            modelBuilder.Entity<Character>().Property(ch => ch.CreationDateTime).HasDefaultValueSql("date('now')");

            modelBuilder.Entity<Character>().HasData(
                new Character()
                {
                    Id = 1,
                    Name = "Luke",
                    Health = 100,
                    Strength = 10,
                    Intelligence = 20,
                    UserId = 1
                },
                new Character()
                {
                    Id = 2,
                    Name = "Gendalf",
                    Health = 100,
                    Strength = 20,
                    Intelligence = 50,
                    UserId = 1
                },
                new Character()
                {
                    Id = 3,
                    Name = "Dart Vader",
                    Health = 100,
                    Strength = 30,
                    Intelligence = 10,
                    UserId = 2
                });

            // Seeding CharacterSkills
            modelBuilder.Entity<CharacterSkill>().HasData(
                new CharacterSkill() { CharacterId = 1, SkillId = 1 },
                new CharacterSkill() { CharacterId = 1, SkillId = 2 },
                new CharacterSkill() { CharacterId = 1, SkillId = 3 },
                new CharacterSkill() { CharacterId = 2, SkillId = 2 },
                new CharacterSkill() { CharacterId = 2, SkillId = 3 },
                new CharacterSkill() { CharacterId = 3, SkillId = 1 });

            // Seeding Weapons
            modelBuilder.Entity<Weapon>().HasData(
                new Weapon() { Id = 1, Name = "FireStick", Damage = 10, CharacterId = 1 },
                new Weapon() { Id = 2, Name = "Axe", Damage = 20, CharacterId = 2 },
                new Weapon() { Id = 3, Name = "Sward", Damage = 20, CharacterId = 3 });

            // Seeding Skills
            modelBuilder.Entity<Skill>().HasData(
                new Skill() { Id = 1, Name = "FireBall", Damage = 50 },
                new Skill() { Id = 2, Name = "Blizzard", Damage = 100 },
                new Skill() { Id = 3, Name = "Frenzy", Damage = 70 });

            // Seeding Role
            modelBuilder.Entity<Role>().HasData(
                new Role() { Id = 1, Name = "Admin" },
                new Role() { Id = 2, Name = "Player" });

            // Seeding default User
            modelBuilder.Entity<User>().HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>().Property(u => u.RoleId).HasDefaultValue(2);

            Utility.CreatePasswordHash("123456", out byte[] passwordSalt, out byte[] passwordHash);

            modelBuilder.Entity<User>().HasData(
                new User() { Id = 1, Name = "Anton", PasswordSalt = passwordSalt, PasswordHash = passwordHash, RoleId = 2 },
                new User() { Id = 2, Name = "Roger", PasswordSalt = passwordSalt, PasswordHash = passwordHash, RoleId = 2 });
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
