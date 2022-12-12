using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_webapi_rpg.Data;
using dotnet_webapi_rpg.DTOs.Character;
using dotnet_webapi_rpg.DTOs.Weapon;
using Microsoft.EntityFrameworkCore;

namespace dotnet_webapi_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try
            {
                // Take the character for which the weapon is created for
                Character character = await _context.Characters.FirstOrDefaultAsync(
                    c => c.Id == newWeapon.CharacterId && 
                    c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                );

                if(character == null) {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                // Map the add weapon dto to to a weapon
                Weapon weapon = new Weapon();
                _mapper.Map(newWeapon, weapon);

                // Add weapon to the database
                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}