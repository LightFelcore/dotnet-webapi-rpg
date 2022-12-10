using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_webapi_rpg.DTOs.User;
using dotnet_webapi_rpg.Services.AuthRepository;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_rpg.Data
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request) 
        {
            var response = await _authRepo.Register(
                new User{ Username = request.Username },
                request.Password
            );
            
            // Check the response
            if(!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<String>>> Login(UserLoginDto request) 
        {
            var response = await _authRepo.Login(request.Username, request.Password);
            
            // Check the response
            if(!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}