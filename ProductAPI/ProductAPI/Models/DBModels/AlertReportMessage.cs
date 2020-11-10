using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.DBModels
{
    public class AlertReportMessage
    {
        [Key]
        public int ReportID { get; set; }
        [Required]
        public DateTime ReportDate { get; set; }
        [Column(TypeName = "varchar(255)")]
        [Required]
        public String AlertReportObj { get; set; }
    }
}
