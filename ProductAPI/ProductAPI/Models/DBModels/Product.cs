using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductAPI.Models.ViewModel;

namespace ProductAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string ProductName { get; set; }
        [Column(TypeName = "float(50)")]
        [Required]
        public float UnitPrice { get; set; }
        [Column(TypeName = "int")]
        [Required]
        public int ProductTypeID { get; set; }
        public ProductType ProductTypeName { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string ImageName { get; set; }


        public IList<Stock> Stocks { get; set; }
    }
}
