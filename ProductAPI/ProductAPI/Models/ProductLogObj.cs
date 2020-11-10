using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class ProductLogObj
    {
        public int ProductID { get; set; }
        
        public string ProductName { get; set; }
        
        public float UnitPrice { get; set; }
       
        public int ProductTypeID { get; set; }

        public string ProductTypeName { get; set; }

        public string ProductTypeDescription { get; set; }

        public string ImageName { get; set; }
    }
}
