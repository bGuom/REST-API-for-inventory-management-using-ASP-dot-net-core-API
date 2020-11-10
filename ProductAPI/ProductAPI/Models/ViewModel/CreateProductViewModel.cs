using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.ViewModel
{
    public class CreateProductViewModel
    {

       
        
        [Required]
        public string ProductName { get; set; }

        [Required]
        public float UnitPrice { get; set; }

        [Required]
        public int ProductTypeID { get; set; }

        public IFormFile Image { get; set; }

    }
}
