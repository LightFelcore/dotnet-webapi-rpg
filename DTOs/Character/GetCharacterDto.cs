using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_webapi_rpg.DTOs.Skill;
using dotnet_webapi_rpg.DTOs.Weapon;

namespace dotnet_webapi_rpg.DTOs.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public String Name { get; set; } = "Fodo";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Knight;
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}