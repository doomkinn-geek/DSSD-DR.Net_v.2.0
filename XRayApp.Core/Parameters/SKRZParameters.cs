using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace XRayApp.Core.Parameters
{
    public class SKRZParameters
    {
        [XmlElement("ImageFolderPath")]
        public string ImageFolderPath { get; set; }

        public static SKRZParameters FromXml(string xmlFilePath)
        {
            using (var reader = new System.IO.StreamReader(xmlFilePath))
            {
                var serializer = new XmlSerializer(typeof(SKRZParameters));
                return (SKRZParameters)serializer.Deserialize(reader);
            }
        }

        public void ToXml(string xmlFilePath)
        {
            using (var writer = new System.IO.StreamWriter(xmlFilePath))
            {
                var serializer = new XmlSerializer(typeof(SKRZParameters));
                serializer.Serialize(writer, this);
            }
        }
    }
}
