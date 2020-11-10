using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class ApplicationSettings
    {

        public string JWT_Secret { get; set; }
        public string ProductImageStoragePath { get; set; }

        public string baseURL { get; set; }
    }
}
