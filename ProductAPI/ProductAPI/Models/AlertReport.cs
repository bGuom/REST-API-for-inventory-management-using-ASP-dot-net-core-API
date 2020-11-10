using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class AlertReport
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public List<int> StockIDs { get; set; }
    }
}
