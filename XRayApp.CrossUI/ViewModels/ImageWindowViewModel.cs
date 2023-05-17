using Avalonia.Media.Imaging;
using ReactiveUI;
using XRayApp.CrossUI.Services;

namespace XRayApp.CrossUI.ViewModels
{
    public class ImageWindowViewModel : ReactiveObject
    {
        private Bitmap _imageSource;
        private readonly ImageService _imageService;
        private string _imageFilePath;

        public ImageWindowViewModel(string filePath)
        {
            _imageService = new ImageService();
            OpenImage(filePath);
        }

        public Bitmap ImageSource
        {
            get => _imageSource;
            set => this.RaiseAndSetIfChanged(ref _imageSource, value);
        }

        public void OpenImage(string filePath)
        {
            ImageSource = _imageService.LoadImage(filePath);
        }
    }
}
