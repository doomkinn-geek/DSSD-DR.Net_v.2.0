using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RentgenArmParser.Models
{
    [Table("pictures")]
    public class ARMPicture
    {
        public int id_picture { get; set; }
        public int id_patient { get; set; }
        public int? id_study { get; set; }
        public DateTime date_p { get; set; }
        public TimeSpan? time_p { get; set; }
        public string? discr { get; set; }
        public string file_p { get; set; }
        public byte[]? mini_p { get; set; }
        public int? id_mode { get; set; }
        public bool deleted { get; set; }
        public int? id_user { get; set; }
        public string? series_UID { get; set; }
        public string? sop_inst_UID { get; set; }
        public int? PositionIndex { get; set; }
        public bool checked_dcm_store { get; set; }
        public uint? kv { get; set; } // было byte, стало uint?
        public float? ma { get; set; } // стало float?
        public float? ms { get; set; } // стало float?
        public uint? SID { get; set; } // было byte, стало uint?
        public float? dose { get; set; }
        public float? effective_dose { get; set; }
        public bool stitch { get; set; }
    }


}
