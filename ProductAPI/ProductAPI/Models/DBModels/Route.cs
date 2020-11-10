using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class Route
    {
        [Key]
        public int RouteID { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Required]
        public string RouteArray { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Description { get; set; }

        public IList<StockDelivery> AllStockDeliveries { get; set; }
    }
}
