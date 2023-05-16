using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using XRayApp.Core.File;
using XRayApp.UI.Services;

namespace XRayApp.UI.ViewModel
{
    public class ImageWindowViewModel : BaseViewModel
    {
        private ImageSource _imageSource;
        private readonly ImageService _imageService;
        private string _imageFilePath;

        public ImageWindowViewModel(string filePath)
        {
            _imageService = new ImageService();
            OpenImage(filePath);
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public void OpenImage(string filePath)
        {
            ImageSource = _imageService.LoadImage(filePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
