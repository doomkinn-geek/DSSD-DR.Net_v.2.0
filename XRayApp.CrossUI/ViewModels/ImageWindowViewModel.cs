using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System;
using XRayApp.CrossUI.Services;
using XRayApp.Core.File;
using XRayApp.Core;

namespace XRayApp.CrossUI.ViewModels
{
    public class ImageWindowViewModel : ReactiveObject
    {
        private Bitmap _imageSource;
        private SrfFileData _originalSrfData; // новое поле для хранения исходных данных
        private readonly ImageService _imageService;
        private string _imageFilePath;

        private string _brightness;
        private string _contrast;
        private bool _isNegative;

        public string Brightness
        {
            get => _brightness;
            set
            {
                this.RaiseAndSetIfChanged(ref _brightness, value);
            }
        }

        public string Contrast
        {
            get => _contrast;
            set
            {
                this.RaiseAndSetIfChanged(ref _contrast, value);
            }
        }

        public bool IsNegative
        {
            get => _isNegative;
            set
            {
                if (value != _isNegative)
                {
                    _isNegative = value;
                    this.RaiseAndSetIfChanged(ref _isNegative, value);
                    UpdateImage();
                    ApplyChanges(); // Применение изменений при изменении свойства IsNegative
                }
            }
        }


        // Команда для применения изменений
        public ReactiveCommand<Unit, Unit> ApplyChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> ApplySobelCommand { get; }

        public ImageWindowViewModel(string filePath)
        {
            _imageService = new ImageService();
            OpenImage(filePath);

            // Инициализируем команду
            ApplyChangesCommand = ReactiveCommand.Create(ApplyChanges);
            ApplySobelCommand = ReactiveCommand.Create(ApplySobel);
        }

        public Bitmap ImageSource
        {
            get => _imageSource;
            set => this.RaiseAndSetIfChanged(ref _imageSource, value);
        }

        private void ApplySobel()
        {
            var sobelResult = ImageProcessor.ApplySobelFilter(_originalSrfData);
            ImageSource = _imageService.MakeImage(sobelResult);
        }

        private void ApplyChanges()
        {
            if (int.TryParse(_brightness, out var brightnessValue) && int.TryParse(_contrast, out var contrastValue))
            {
                _originalSrfData.Brightness = brightnessValue;
                _originalSrfData.Contrast = contrastValue;
                _originalSrfData.IsNegative = _isNegative;
                UpdateImage();
            }
            else
            {
                // Можно добавить обработку ошибок ввода здесь
            }
        }

        public void OpenImage(string filePath)
        {
            _originalSrfData = _imageService.LoadSrfData(filePath); // Загружаем исходные данные
            Brightness = _originalSrfData.Brightness.ToString();
            Contrast = _originalSrfData.Contrast.ToString();
            IsNegative = _originalSrfData.IsNegative;
            UpdateImage();
        }

        // Метод для обновления изображения
        private void UpdateImage()
        {
            ImageSource = _imageService.MakeImage(_originalSrfData);
        }
    }


}
