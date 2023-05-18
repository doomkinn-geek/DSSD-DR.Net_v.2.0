using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayApp.Data.Models
{
    // Класс модели Study
    [Table("studies")]
    public class Study
    {
        public int Id { get; set; } // Primary Key
        public string? StudyId { get; set; } // Study ID
        public DateTime StudyDate { get; set; } // Date of the study
        public string? Description { get; set; } // Description of the study

        // Foreign Key for Patient
        public int PatientId { get; set; } // Foreign key for Patient
        public Patient Patient { get; set; } // Navigation property

        // Navigation property
        public ICollection<Image> Images { get; set; } // Collection of Images related to the study
    }
}
