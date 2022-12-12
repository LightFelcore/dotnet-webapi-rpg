using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_webapi_rpg.DTOs.Fight
{
    public class AttackResultDto
    {
        public String AttackerName { get; set; } = String.Empty;
        public String OpponentName { get; set; } = String.Empty;
        public int AttackerHitPoints { get; set; }
        public int OpponentHitPoints { get; set; }
        public int Damage { get; set; }
    }
}