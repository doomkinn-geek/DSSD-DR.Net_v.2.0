using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentgenArmParser.Models
{
    [Table("study")]
    public class ARMStudy
    {
        public int id_study { get; set; }
        public int? id_patient { get; set; }
        public string? study_UID { get; set; }
        public string? series_UID { get; set; }
        public string? StudyID { get; set; }
        public string? StudyDiscr { get; set; }
        public DateTime? date_study { get; set; }
        public TimeSpan? time_study { get; set; }
        public byte? deleted { get; set; }
        public int? id_doctor { get; set; }
        public int? id_study_type { get; set; }
        public byte? checked_dcm_store { get; set; }
        public string? pre_diagnosis { get; set; }
        public string? directed { get; set; }
        public string? accessionNum { get; set; }
    }
}
