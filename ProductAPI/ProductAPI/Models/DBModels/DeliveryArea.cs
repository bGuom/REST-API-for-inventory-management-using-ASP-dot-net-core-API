using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class DeliveryArea
    {
        [Key]
        public int DeliveryID { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Required]
        public string PolyArray { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Description { get; set; }

        public IList<StockDelivery> AllStockDeliveries { get; set; }
    }
}
