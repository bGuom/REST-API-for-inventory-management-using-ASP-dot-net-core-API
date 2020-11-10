using Microsoft.AspNetCore.Identity;
using ProductAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        
    }
}
