using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentgenArmParser.Models
{
    [Table("patient")]
    public class ARMPatient
    {
        public int id_patient { get; set; }
        public string? PatientID { get; set; }
        public string? surname { get; set; }
        public string? name { get; set; }
        public string? fathername { get; set; }
        public DateTime birthday { get; set; }
        public string? sex { get; set; }
        public string? ambulance_card { get; set; }
        public string? contingent { get; set; }
        public string? risk_group { get; set; }
        public string? uchastok { get; set; }
        public string? policy { get; set; }
        public string? pasport { get; set; }
        public string? address { get; set; }
        public string? telephone { get; set; }
        public string? profession { get; set; }
        public string? complaints { get; set; }
        public string? diagnosis { get; set; }
        public string? comment { get; set; }
        public byte? pathology { get; set; }
        public byte? flag { get; set; }
    }

}
