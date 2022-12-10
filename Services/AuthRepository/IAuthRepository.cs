using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_webapi_rpg.Services.AuthRepository
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, String password);
        Task<ServiceResponse<String>> Login(String username, String password);
        Task<bool> UserExists(String username);
    }
}