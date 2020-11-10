using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class JWT
    {
        public const string Iss = "TSP";
        public const string Aud = "ApiUser";
       
        public const string AuthScheme = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
