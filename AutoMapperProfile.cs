using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_webapi_rpg.DTOs.Character;
using dotnet_webapi_rpg.DTOs.Fight;
using dotnet_webapi_rpg.DTOs.Skill;
using dotnet_webapi_rpg.DTOs.Weapon;

namespace dotnet_webapi_rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Characters
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<UpdatedCharacterDto, Character>();

            // Weapons
            CreateMap<AddWeaponDto, Weapon>();
            CreateMap<Weapon, GetWeaponDto>();

            // Skills
            CreateMap<Skill, GetSkillDto>();

            // HighScores
            CreateMap<Character, HighScoreDto>();
        }
    }
}