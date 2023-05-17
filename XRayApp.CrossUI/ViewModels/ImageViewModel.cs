using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XRayApp.Core;
using XRayApp.CrossUI.ViewModels;
using XRayApp.Data;
using XRayApp.Data.Models;


namespace XRayApp.CrossUI.ViewModels
{
    public class ImageViewModel : ReactiveObject
    {
        private readonly DatabaseManager _databaseService;
        private Image _selectedImage;

        public ObservableCollection<Image> Images { get; private set; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ViewImageCommand { get; private set; }

        public ImageViewModel(DatabaseManager databaseService, StudyViewModel studyViewModel)
        {
            _databaseService = databaseService;

            // Подпишитесь на событие SelectedStudyChanged в StudyViewModel.
            studyViewModel.SelectedStudyChanged += LoadImages;

            // Использование ReactiveCommand
            DeleteCommand = ReactiveCommand.Create(DeleteImage, this.WhenAnyValue(x => x.SelectedImage).Select(x => x != null));
            ViewImageCommand = ReactiveCommand.Create(ViewImage, this.WhenAnyValue(x => x.SelectedImage).Select(x => x != null));
        }

        public Image SelectedImage
        {
            get { return _selectedImage; }
            set { this.RaiseAndSetIfChanged(ref _selectedImage, value); }
        }

        private void DeleteImage()
        {
            _databaseService.ImagesRepository.DeleteImage(SelectedImage.Id);
            _databaseService.SaveChanges();
        }

        private void ViewImage()
        {
            var window = new ImageWindow();
            var viewModel = new ImageWindowViewModel(SelectedImage.ImagePath);
            window.DataContext = viewModel;
            window.Show();
        }

        public void LoadImages(Study selectedStudy)
        {
            if (selectedStudy == null) return;
            if (selectedStudy.Id != 0)
            {
                Images = new ObservableCollection<Image>(_databaseService.ImagesRepository.GetImagesByStudyId(selectedStudy.Id));
            }
            else
            {
                Images = new ObservableCollection<Image>();
            }
            this.RaisePropertyChanged(nameof(Images));
        }
    }


}
