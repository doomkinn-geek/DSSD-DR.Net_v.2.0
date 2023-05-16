using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XRayApp.Core.File;
using XRayApp.Core.Parameters;
using XRayApp.UI.ViewModel;

namespace XRayApp.UI.Services
{
    public class ImageService
    {
        public ImageSource LoadImage(string filePath)
        {
            var exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var xmlFilePath = Path.Combine(exeDirectory, "parameters.xml");

            var parameters = SKRZParameters.FromXml(xmlFilePath);
            var imageFolderPath = parameters.ImageFolderPath;

            var fullPath = Path.Combine(imageFolderPath, filePath);
            fullPath += ".srf";
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("File path must not be empty", nameof(filePath));
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            // Load SRF file data
            var reader = new SrfFileReader(fullPath);
            var srfData = reader.ReadSrfFile();
            //srfData.IsNegative = !srfData.IsNegative;

            // Assuming srfFileData.PixelData is a one-dimensional array representing the image.
            // Note that we'll need to apply brightness/contrast adjustments and convert ushort to byte.
            var stride = srfData.FrameWidth * 2; // 2 bytes per pixel

            ushort[] harmonizedData = HarmonizeImage(srfData.PixelData, srfData.Brightness, srfData.Contrast);

            // Apply brightness and contrast adjustments here before creating the BitmapSource.
            ushort[] adjustedData = ApplyBrightnessAndContrast(harmonizedData, srfData.Brightness, srfData.Contrast, srfData.IsNegative);

            int targetWidth = srfData.FrameWidth / 2; // adjust as necessary
            int targetHeight = srfData.FrameHeight / 2; // adjust as necessary

            byte[] pixelData = DownsamplePixelData(adjustedData, srfData.FrameWidth, srfData.FrameHeight, targetWidth, targetHeight);

            /*string fileName = $"PixelData_{targetWidth}x{targetHeight}.raw";
            try
            {
                File.WriteAllBytes(fileName, pixelData);
                Console.WriteLine("Pixel data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the pixel data: {ex.Message}");
            }*/

            ImageSource image = BitmapSource.Create(
                targetWidth,
                targetHeight,
                96, 96, // DPI settings, adjust as necessary
                PixelFormats.Gray16, // assuming grayscale image
                null,
                pixelData,
                targetWidth * 2 // new stride
                );


            return image;
        }
        public ushort[] HarmonizeImage(ushort[] sourceData, int brightness, int contrast)
        {
            int length = sourceData.Length;
            ushort[] result = new ushort[length];

            ushort MinPixValue = 32767;
            ushort MaxPixValue = 0;

            ushort MinHisValue;
            ushort MaxHisValue;

            float k;
            float black;
            int i = 0;

            long[] histogram = new long[32768];

            for (i = 0; i < length; i++)
            {
                ushort CurrentValue = sourceData[i];

                // LIMITING value to 14 bit
                if (CurrentValue < 0) { CurrentValue = 0; }
                if (CurrentValue > 16383) { CurrentValue = 16383; }

                // *****Max/Min value calculation*****
                if (CurrentValue > MaxPixValue) { MaxPixValue = CurrentValue; }
                if (CurrentValue < MinPixValue) { MinPixValue = CurrentValue; }

                // *****Histogram building*****
                histogram[CurrentValue]++;
            }

            // Histogram DownSearch
            for (i = 16383; i >= 0; i--)
            {
                if (histogram[i] > 100) { break; }
            }

            MaxHisValue = (ushort)i;

            // Histogram UpSearch
            for (i = 0; i <= 16383; i++)
            {
                if (histogram[i] > 100) { break; }
            }

            MinHisValue = (ushort)i;

            // Calculation of Image Dynamic
            int different = MaxHisValue - MinHisValue;
            if (different == 0) { different = 1; } // prevent division by 0

            // Calculation of constant multiplier for this dynamic
            k = (float)(16383.00 / different);

            // Set black level
            black = (float)MinHisValue;

            for (i = 0; i < length; i++)
            {
                float inpix = (float)sourceData[i];

                // black level, convert. dynRange
                float outpix = ((inpix - black) * k);

                // limiting 
                if (outpix > 16383.00) { outpix = 16383; }
                if (outpix < 0.00) { outpix = 0; }

                result[i] = (ushort)outpix;
            }

            return result;
        }


        public byte[] DownsamplePixelData(ushort[] sourceData, int originalWidth, int originalHeight, int targetWidth, int targetHeight)
        {
            ushort[] downsampledData = new ushort[targetWidth * targetHeight];

            float xRatio = (float)originalWidth / targetWidth;
            float yRatio = (float)originalHeight / targetHeight;

            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    int px = (int)Math.Floor(x * xRatio);
                    int py = (int)Math.Floor(y * yRatio);

                    downsampledData[y * targetWidth + x] = sourceData[py * originalWidth + px];
                }
            }

            // Преобразуйте данные ushort в байты
            int length = downsampledData.Length;
            byte[] result = new byte[length * 2]; // каждый пиксель кодируется 2 байтами

            for (int i = 0; i < length; i++)
            {
                result[2 * i] = (byte)(downsampledData[i] & 0xFF); // младший байт
                result[2 * i + 1] = (byte)((downsampledData[i] >> 8) & 0xFF); // старший байт
            }

            return result;
        }

        public ushort[] ApplyBrightnessAndContrast(ushort[] sourceData, int brightness, int contrast, bool negative = false)
        {
            int length = sourceData.Length;
            ushort[] result = new ushort[length];

            float mBrightness = (float)brightness;
            float mContrast = (float)contrast;
            float medium = 8191;
            float contrastFactor = (mContrast + medium) / medium;
            float mB = medium + mBrightness;

            Parallel.For(0, length, i =>
            {
                // Применение коррекции яркости и контраста
                float oldBrightness = sourceData[i];

                // Применение инверсии, если negative равно true
                if (negative)
                {
                    oldBrightness = 16383 - oldBrightness;
                }

                float newBrightness = ((oldBrightness - medium) * contrastFactor) + mB;

                // Ограничение значения пикселя диапазоном [0, 65535]
                ushort newValue = (ushort)Math.Max(0, Math.Min(65535, (int)newBrightness));

                result[i] = newValue;
            });

            return result;
        }


    }
}
