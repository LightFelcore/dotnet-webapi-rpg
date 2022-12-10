using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnet_webapi_rpg.Models;
using dotnet_webapi_rpg.Services.CharacterService;
using dotnet_webapi_rpg.DTOs.Character;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotnet_webapi_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // [AllowAnonymous]  allows 'get characters' even if anonymous. All the other methods need autorization in header request
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetCharacters() {
            // Grab the user id in order to get all the characters for a specific user
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _characterService.GetCharacters(userId));
        }
        
        [HttpGet("{id}")] // send id through the URL
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetCharacterById(int id) {
            var response = await _characterService.GetCharacterById(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpPost] // send the json new character through the body
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> CreateCharacter(AddCharacterDto character) {
            return Ok(await _characterService.CreateCharacter(character));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdatedCharacterDto updatedCharacter) {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int id) {
            var response = await _characterService.DeleteCharacter(id);
            if(response.Data == null) return NotFound(response);
            return Ok(response);
        }
    }
}