using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class StockDelivery
    {
        [Key]
        public int DeliveryID { get; set; }
        [Required]
        public int StockID { get; set; }
        [Required]
        public int SalesRepID { get; set; }
        public SalesRep SalesRep { get; set; }
        [Column(TypeName = "date")]
        [Required]
        public DateTime DeliveryDate { get; set; }
        [Column(TypeName = "varchar(25)")]
        [Required]
        public string Status { get; set; }
        [Required]
        public int DeliveryAreaID { get; set; }
        public DeliveryArea DeliveryArea { get; set; }
        [Required]
        public int RouteID { get; set; }
        public Route Route { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string Notes { get; set; }

        public IList<Stock> AllStocks { get; set; }


    }
}
