using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayApp.Data.Models
{
    // Класс модели Image
    public class Image
    {
        public int Id { get; set; } // Primary Key
        public string? ImageId { get; set; } // Image ID
        public DateTime ImageDate { get; set; } // Date of the image
        public string? ImagePath { get; set; } // Path where image file is stored
        public string? SeriesUID { get; set; } // DICOM Series UID
        public string? StudyUID { get; set; } // DICOM Study UID
        public string? ExposureParameters { get; set; } // Exposure parameters for X-ray image

        // Foreign Key for Study
        public int StudyId { get; set; } // Foreign key for Study
        public Study Study { get; set; } // Navigation property
    }
}
