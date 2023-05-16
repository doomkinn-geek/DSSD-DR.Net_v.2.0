using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayApp.Core
{
    public class Examination
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public string DicomStudyUID { get; set; }

        public List<XRayImage> Images { get; set; } = new List<XRayImage>();
    }

}
