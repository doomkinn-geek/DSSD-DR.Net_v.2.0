using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using XRayApp.Core.File;

namespace XRayApp.UI.ViewModel
{
    public class ImageWindowViewModel : BaseViewModel
    {
        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadImageData(SrfFileData srfFileData)
        {
            // Assuming srfFileData.PixelData is a one-dimensional array representing the image.
            // Note that we'll need to apply brightness/contrast adjustments and convert ushort to byte.
            var stride = srfFileData.FrameWidth * 2; // 2 bytes per pixel

            // Apply brightness and contrast adjustments here before creating the BitmapSource.
            byte[] pixelData = ConvertPixelData(srfFileData.PixelData, srfFileData.Brightness, srfFileData.Contrast);

            ImageSource = BitmapSource.Create(
                srfFileData.FrameWidth,
                srfFileData.FrameHeight,
                96, 96, // DPI settings, adjust as necessary
                PixelFormats.Gray16, // assuming grayscale image
                null,
                pixelData,
                stride
            );
        }

        private byte[] ConvertPixelData(ushort[] sourceData, int brightness, int contrast)
        {
            // Convert ushort pixel data to byte array with applied brightness and contrast.
            // This is a placeholder, you'll need to fill this with the actual algorithm.
            return new byte[sourceData.Length];
        }
    }
}
