using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayApp.Core
{
    public class XRayImage
    {
        public XRayImage(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Id { get; set; }
        public int ExaminationId { get; set; }
        public Examination Examination { get; set; }
        public DateTime DateTime { get; set; }
        public string DicomSeriesUID { get; set; }
        public string DicomStudyUID { get; set; }
        public string XRayExposureParameters { get; set; }
        public ushort[] ImageData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
