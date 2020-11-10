using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.ViewModel
{
    public class ProductType
    {
        [Key]
        public int ProductTypeID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string TypeName { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string Description { get; set; }


        public IList<Product> Products { get; set; }
    }
}
