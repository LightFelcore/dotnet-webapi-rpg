using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_webapi_rpg.DTOs.User
{
    public class UserRegisterDto
    {
        public String Username { get; set; } = String.Empty;
        public String Password { get; set; } = String.Empty;
    }
}