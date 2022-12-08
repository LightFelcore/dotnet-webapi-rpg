using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_webapi_rpg.DTOs.Character;

namespace dotnet_webapi_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper) {
            _mapper = mapper;
        }
        
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam" }
        };

        public async Task<ServiceResponse<List<GetCharacterDto>>> CreateCharacter(AddCharacterDto newCharacter)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            // Map AddCharacterDto to Character
            Character character = _mapper.Map<Character>(newCharacter);
            // Auto increment the id by taking the last id in the list and adding 1
            character.Id = characters.Max(c => c.Id) + 1;
            characters.Add(character);
            // Map every character of type Character in the list to a GetCharacterDto
            response.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            
            try
            {
                response.Data = _mapper.Map<GetCharacterDto>(characters.First(c => c.Id == id));
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
            return new ServiceResponse<List<GetCharacterDto>>{ Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList() };
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdatedCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
           
            try
            {
                Character character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);

                // Override ALL the character properties. In case you want to change only a few propoerties, you can
                // do so;ething like `characted.Name = updatedCharacter.Namen`, and so on
                _mapper.Map(updatedCharacter, character);

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
                Character character = characters.First(c => c.Id == id);
                characters.Remove(character);
                response.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
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