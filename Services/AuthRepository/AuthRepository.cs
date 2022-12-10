using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_webapi_rpg.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_webapi_rpg.Services.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();

            // Find the correct user by username - returns either the user or null
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

            // Check if the user was found
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            // Verify if the user entred a correct password
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
            }
            // User is authenticated
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();

            // Check if user already exists
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            // Generate password hash and salt
            CreatePasswordHashAndSalt(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Add the user to the database
            _context.Users.Add(user);
            // Save the changes to the database
            await _context.SaveChangesAsync();
            // Pass the new registered user id to the Service Response
            response.Data = user.Id;

            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            // Check if user already exists by username - returns either true or false
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        private void CreatePasswordHashAndSalt(String password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Instanciating the password hash alogoritm
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(String password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Instanciating the password hash alogoritm
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // Hash the entred password
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Compare the generated entred password hash with the hash inside the database, BYTE BY BYTE
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        // TODO: Don't forget to add the Authentication Scheme to the Program.cs
        private String CreateToken(User user)
        {
            // Render a list of claims
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Generate a symmetric sucirity key with token set in the app settings
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
            );

            // Create signing credentials for HMACSHA512 security algoritm
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create a security token desciptor
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            // Create a JWT security token handler
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize the token into a JWT and return it as a string
            return tokenHandler.WriteToken(token);
        }
    }
}