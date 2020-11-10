using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class StockAlert
    {
        [Key]
        public int AlertID { get; set; }
        [Required]
        public int StockID { get; set; }
        public Stock Stock { get; set; }
        [Column(TypeName = "varchar(15)")]
        [Required]
        public String AlertType { get; set; }
        [Column(TypeName = "varchar(10)")]
        [Required]
        public string Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        [Column(TypeName = "varchar(10)")]
        [Required]
        public string Status { get; set; }
    }
}
