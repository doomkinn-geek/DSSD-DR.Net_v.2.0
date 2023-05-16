using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayApp.Core.File
{
    public class SrfFileReader
    {
        public SrfFileData ReadSrfFile(string filePath)
        {
            var srfData = new SrfFileData();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                srfData.Prefix = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                srfData.FrameWidth = binaryReader.ReadInt32();
                srfData.FrameHeight = binaryReader.ReadInt32();
                srfData.Contrast = binaryReader.ReadInt32();
                srfData.Brightness = binaryReader.ReadInt32();
                srfData.IsNegative = binaryReader.ReadBoolean();
                binaryReader.ReadBytes(3); // Skip 3 bytes as the Boolean took 4 bytes, but we only used 1
                srfData.ContextVisionLUT = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                srfData.ContextVisionGOP = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));
                srfData.KalimatorLeft = binaryReader.ReadInt16();
                srfData.KalimatorTop = binaryReader.ReadInt16();
                srfData.KalimatorRight = binaryReader.ReadInt16();
                srfData.KalimatorBottom = binaryReader.ReadInt16();
                srfData.BitDepth = binaryReader.ReadByte();
                srfData.PixelSize = binaryReader.ReadInt16();
                srfData.VerticalFlip = binaryReader.ReadByte();
                srfData.HorizontalFlip = binaryReader.ReadByte();
                srfData.RotationDegree = binaryReader.ReadInt16();
                srfData.NormalizationIndex = binaryReader.ReadInt32();
                srfData.MinimalAdjustmentLevel = binaryReader.ReadInt16();
                srfData.OrientationString = Encoding.ASCII.GetString(binaryReader.ReadBytes(4));

                srfData.PixelData = new List<ushort>();
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    srfData.PixelData.Add(binaryReader.ReadUInt16());
                }
            }
            return srfData;
        }
    }
}
