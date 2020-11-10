using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.BindingModel
{
    public class StockUse
    {
        public int StockID { get; set; }
        public int UsedQuantity { get; set; }
    }
}
