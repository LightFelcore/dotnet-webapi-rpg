using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_webapi_rpg.Data;
using dotnet_webapi_rpg.DTOs.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_webapi_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context) {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> CreateCharacter(AddCharacterDto newCharacter)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            // Map AddCharacterDto to Character
            Character character = _mapper.Map<Character>(newCharacter);
            //Fetch caracters from the database and add the new character to the list
            _context.Characters.Add(character);
            //Save the changes to the database
            await _context.SaveChangesAsync();
            // Map every character of type Character in the list to a GetCharacterDto
            response.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            
            try
            {
                var dbCharacter = await _context.Characters.FirstAsync<Character>(c => c.Id == id);
                response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            response.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdatedCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
           
            try
            {
                Character character = await _context.Characters.FirstAsync(c => c.Id == updatedCharacter.Id);

                // Override ALL the character properties. In case you want to change only a few propoerties, you can
                // do something like `characted.Name = updatedCharacter.Namen`, and so on
                _mapper.Map(updatedCharacter, character);

                // save the updated character to the database
                await _context.SaveChangesAsync();

                // Map the Character to a GetCharacter Dto
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character character = await _context.Characters.FirstAsync(c => c.Id == id);
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
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
