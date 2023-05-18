using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Core.File;

namespace XRayApp.Core
{
    public static class ImageProcessor
    {
        public static XRayImage NormalizeHistogram(XRayImage image)
        {
            // 1. Найти минимальное и максимальное значение пикселей в изображении
            ushort minPixelValue = image.ImageData.Min();
            ushort maxPixelValue = image.ImageData.Max();

            // 2. Определить коэффициент нормализации
            double normalizationFactor = 1023.0 / (maxPixelValue - minPixelValue);

            // 3. Применить нормализацию ко всем пикселям изображения
            for (int i = 0; i < image.ImageData.Length; i++)
            {
                int normalizedPixelValue = (int)((image.ImageData[i] - minPixelValue) * normalizationFactor);
                image.ImageData[i] = (ushort)normalizedPixelValue;
            }

            return image;
        }

        public static XRayImage ApplyGammaCorrection(XRayImage image, double gamma)
        {
            // Значения, используемые для преобразования гамма-коррекции
            double inverseGamma = 1.0 / gamma;
            ushort maxIntensity = 1023; // Максимальная интенсивность пикселя для 10-битного изображения

            // Гамма-коррекция для каждого пикселя в изображении
            for (int i = 0; i < image.ImageData.Length; i++)
            {
                double normalizedPixelValue = image.ImageData[i] / (double)maxIntensity;
                double correctedPixelValue = Math.Pow(normalizedPixelValue, inverseGamma) * maxIntensity;
                image.ImageData[i] = (ushort)Math.Round(correctedPixelValue);
            }

            return image;
        }

        public static XRayImage ApplySobelFilter(XRayImage image)
        {
            int width = image.Width;
            int height = image.Height;

            // Кернелы фильтра Собеля
            int[,] gx = new int[3, 3]
            {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
            };

            int[,] gy = new int[3, 3]
            {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
            };

            // Создаем новое изображение, чтобы не модифицировать исходное
            XRayImage result = new XRayImage(width, height);

            // Применяем фильтр Собеля
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int sumX = 0;
                    int sumY = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int pixel = image.ImageData[(y + j) * width + (x + i)];
                            sumX += gx[j + 1, i + 1] * pixel;
                            sumY += gy[j + 1, i + 1] * pixel;
                        }
                    }

                    int sum = Math.Abs(sumX) + Math.Abs(sumY);

                    // Контролируем переполнение и нормализуем до 10-битного значения
                    sum = sum > 1023 ? 1023 : sum;
                    sum = sum < 0 ? 0 : sum;

                    result.ImageData[y * width + x] = (ushort)sum;
                }
            }

            return result;
        }

        public static SrfFileData ApplySobelFilter(SrfFileData data)
        {
            int width = data.FrameWidth;
            int height = data.FrameHeight;

            int[,] gx = new int[3, 3]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            int[,] gy = new int[3, 3]
            {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            ushort[] resultPixelData = new ushort[width * height];

            Parallel.For(1, width - 1, x =>
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int sumX = 0;
                    int sumY = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int pixel = data.PixelData[(y + j) * width + (x + i)];
                            sumX += gx[j + 1, i + 1] * pixel;
                            sumY += gy[j + 1, i + 1] * pixel;
                        }
                    }

                    int sum = Math.Abs(sumX) + Math.Abs(sumY);

                    // Нормализуем до 16-битного значения
                    //sum = sum > 65535 ? 65535 : sum;
                    sum = sum > 16383 ? 16383 : sum;
                    sum = sum < 0 ? 0 : sum;

                    resultPixelData[y * width + x] = (ushort)sum;
                }
            });

            // Создаем новый объект SrfFileData для результата и заполняем его свойства
            SrfFileData result = new SrfFileData
            {
                Prefix = data.Prefix,
                FrameWidth = data.FrameWidth,
                FrameHeight = data.FrameHeight,
                Contrast = data.Contrast,
                Brightness = data.Brightness,
                IsNegative = data.IsNegative,
                ContextVisionLUT = data.ContextVisionLUT,
                ContextVisionGOP = data.ContextVisionGOP,
                KalimatorLeft = data.KalimatorLeft,
                KalimatorTop = data.KalimatorTop,
                KalimatorRight = data.KalimatorRight,
                KalimatorBottom = data.KalimatorBottom,
                BitDepth = data.BitDepth,
                PixelSize = data.PixelSize,
                VerticalFlip = data.VerticalFlip,
                HorizontalFlip = data.HorizontalFlip,
                RotationDegree = data.RotationDegree,
                NormalizationIndex = data.NormalizationIndex,
                MinimalAdjustmentLevel = data.MinimalAdjustmentLevel,
                OrientationString = data.OrientationString,
                PixelData = resultPixelData
            };

            return result;
        }


    }

}
