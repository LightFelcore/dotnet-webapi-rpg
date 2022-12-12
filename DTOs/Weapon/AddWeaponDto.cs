using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_webapi_rpg.DTOs.Weapon
{
    public class AddWeaponDto
    {
        public String Name { get; set; } = String.Empty;
        public int Damage { get; set; }
        public int CharacterId { get; set; }
    }
}