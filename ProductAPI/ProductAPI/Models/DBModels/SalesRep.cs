using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class SalesRep
    {
        [Key]
        public int RepID { get; set; }
        [Required]
        public int UserID { get; set; }

        public IList<StockDelivery> AllDeliveries { get; set; }

    }
}
