using ProductAPI.Models.DBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class Stock
    {
        [Key]
        [Required]
        public int StockID { get; set; }
        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int DeliveryID { get; set; }
        public StockDelivery StockDelivery { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int Threshold { get; set; }
        [Column(TypeName = "date")]
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Supplier { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Description { get; set; }

        public IList<StockAlert> AllAlerts { get; set; }
    }
}
