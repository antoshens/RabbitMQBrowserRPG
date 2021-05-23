using System;
using System.ComponentModel.DataAnnotations;

namespace FightServiceAPI.Model
{
    public class AttackLog
    {
        [Key]
        public int Id { get; set; }
        public int AttackerId { get; set; }
        public int OpponentId { get; set; }
        public int Damage { get; set; }
    }
}
