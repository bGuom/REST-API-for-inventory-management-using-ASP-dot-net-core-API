using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models
{
    public class LogObject
    {
        [Key]
        public int ID { get; set; }
        public string Model { get; set; }
        public int EntityID { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Object { get; set; }
    }
}
