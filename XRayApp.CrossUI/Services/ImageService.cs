using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using XRayApp.Core.File;
using XRayApp.Core.Parameters;

namespace XRayApp.CrossUI.Services
{
    public class ImageService
    {
        public SrfFileData LoadSrfData(string filePath)
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
            // Предполагая, что у вас есть метод для чтения данных из файла в формате SRF
            SrfFileData srfData = reader.ReadSrfFile();

            // Применение функции HarmonizeImage к исходным данным
            ushort[] harmonizedData = HarmonizeImage(srfData.PixelData);

            // Изменение размера изображения
            // Предположим, что DownsamplePixelData возвращает новые данные и обновленные размеры изображения
            int targetWidth = srfData.FrameWidth / 2; // adjust as necessary
            int targetHeight = srfData.FrameHeight / 2; // adjust as necessary

            ushort[] downsampledData = DownsamplePixelData(harmonizedData, srfData.FrameWidth, srfData.FrameHeight, targetWidth, targetHeight);           

            // Обновление данных в srfData
            srfData.PixelData = downsampledData;
            srfData.FrameWidth = targetWidth;
            srfData.FrameHeight = targetHeight;

            return srfData;
        }

        public Bitmap MakeImage(SrfFileData srfData)
        {
            ushort[] contrastBrightData = ApplyBrightnessAndContrast(srfData.PixelData, srfData.Brightness, srfData.Contrast, srfData.IsNegative);

            byte[] pixelData8Bit = Convert16To8Bit(contrastBrightData);
            byte[] pixel32Data = Convert8BitToBgra32(pixelData8Bit);

            string fileName = $"PixelData_W-{srfData.FrameWidth}xH-{srfData.FrameHeight}.raw";
            try
            {
                byte[] byteArray = new byte[srfData.PixelData.Length * sizeof(ushort)];
                Buffer.BlockCopy(srfData.PixelData, 0, byteArray, 0, byteArray.Length);
                File.WriteAllBytes(fileName, byteArray);
                Console.WriteLine("Pixel data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the pixel data: {ex.Message}");
            }

            var format = PixelFormat.Bgra8888; // Monochrome format (32 bits per pixel)
            var alphaFormat = AlphaFormat.Unpremul; // Adjust the alpha format according to your needs
            var dataPtr = Marshal.AllocHGlobal(pixel32Data.Length);
            Marshal.Copy(pixel32Data, 0, dataPtr, pixel32Data.Length);
            var size = new PixelSize(srfData.FrameWidth, srfData.FrameHeight);
            var dpi = new Vector(96, 96);

            var bitmap = new Bitmap(format, alphaFormat, dataPtr, size, dpi, size.Width * 4); // stride is equal to the width now

            Marshal.FreeHGlobal(dataPtr);
            return bitmap;
        }

        private byte[] Convert8BitToBgra32(byte[] pixelData)
        {
            int pixelCount = pixelData.Length;
            byte[] result = new byte[pixelCount * 4]; // Каждый пиксель будет кодироваться 4 байтами (BGRA)

            for (int i = 0; i < pixelCount; i++)
            {
                byte intensity = pixelData[i]; // Значение интенсивности серого
                result[4 * i] = intensity;     // Blue
                result[4 * i + 1] = intensity; // Green
                result[4 * i + 2] = intensity; // Red
                result[4 * i + 3] = 255;       // Alpha
            }

            return result;
        }


        private byte[] Convert16To8Bit(ushort[] sourceData)
        {
            int length = sourceData.Length;
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (byte)(sourceData[i] >> 8); // Игнорирование младших 8 битов
            }

            return result;
        }


        private ushort[] HarmonizeImage(ushort[] sourceData)
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


        private ushort[] DownsamplePixelData(ushort[] sourceData, int originalWidth, int originalHeight, int targetWidth, int targetHeight)
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

            return downsampledData;

            // Преобразуйте данные ushort в байты
            /*int length = downsampledData.Length;
            byte[] result = new byte[length * 2]; // каждый пиксель кодируется 2 байтами

            for (int i = 0; i < length; i++)
            {
                result[2 * i] = (byte)(downsampledData[i] & 0xFF); // младший байт
                result[2 * i + 1] = (byte)((downsampledData[i] >> 8) & 0xFF); // старший байт
            }

            return result;*/
        }

        private ushort[] ApplyBrightnessAndContrast(ushort[] sourceData, int brightness, int contrast, bool negative = false)
        {
            int length = sourceData.Length;
            ushort[] result = new ushort[length];

            float mBrightness = (float)brightness;
            float mContrast = (float)contrast;
            float medium = 8191;
            float contrastFactor = (mContrast + medium) / medium;
            float mB = medium + mBrightness;

            if (!negative)
            {
                Parallel.For(0, length, i =>
                {
                    float oldBrightness = sourceData[i];
                    float newBrightness = ((oldBrightness - medium) * contrastFactor) + mB;
                    //newBrightness = newBrightness / 64;

                    if (newBrightness > 16383)
                    {
                        newBrightness = 16383;
                    }

                    if (newBrightness < 0)
                    {
                        newBrightness = 0;
                    }

                    result[i] = (ushort)newBrightness;
                });
            }
            else
            {
                Parallel.For(0, length, i =>
                {
                    float oldBrightness = sourceData[i];
                    float newBrightness = (((16383 - oldBrightness) - medium) * contrastFactor) + mB;
                    //newBrightness = newBrightness / 64;

                    if (newBrightness > 16383)
                    {
                        newBrightness = 16383;
                    }

                    if (newBrightness < 0)
                    {
                        newBrightness = 0;
                    }

                    result[i] = (ushort)newBrightness;
                });
            }

            return result;

        }
    }
}
