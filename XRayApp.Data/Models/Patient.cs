using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace XRayApp.Data.Models
{
    // Класс модели Patient
    [Table("patients")]
    public class Patient
    {
        public int Id { get; set; } // Primary Key
        public string PatientId { get; set; } // Patient ID
        public string FirstName { get; set; } // First Name of the patient
        public string LastName { get; set; } // Last Name of the patient
        public string MiddleName { get; set; } // Middle Name of the patient
        public DateTime BirthDate { get; set; } // Date of Birth
        public string Gender { get; set; } // Gender of the patient
        public string? Address { get; set; } // Address of the patient
        public string? Comment { get; set; } // Additional comments

        // Navigation property
        public ICollection<Study> Studies { get; set; } // Collection of Studies related to the patient
    }
}
