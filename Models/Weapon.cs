using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_webapi_rpg.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        public String Name { get; set; } = String.Empty;
        public int Damage { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }
    }
}