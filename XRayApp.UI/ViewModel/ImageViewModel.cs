using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XRayApp.Core;
using XRayApp.Data;
using XRayApp.Data.Models;
using XRayApp.UI.Commands;

namespace XRayApp.UI.ViewModel
{
    public class ImageViewModel : BaseViewModel
    {
        private readonly DatabaseManager _databaseService;
        private Image _selectedImage;
        private RelayCommand _viewImageCommand;

        public ObservableCollection<Image> Images { get; private set; }

        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand ViewImageCommand => _viewImageCommand ??= new RelayCommand(ViewImage);


        public ImageViewModel(DatabaseManager databaseService, StudyViewModel studyViewModel)
        {
            _databaseService = databaseService;

            // Подпишитесь на событие SelectedStudyChanged в StudyViewModel.
            studyViewModel.SelectedStudyChanged += LoadImages;

            DeleteCommand = new RelayCommand(DeleteImage, CanDeleteImage);            
            //ViewImageCommand = new RelayCommand(ViewImage);

            //var images = databaseService.ImagesRepository.GetAll();
            //Images = new ObservableCollection<Image>(images);
        }       

        public Image SelectedImage
        {
            get { return _selectedImage; }
            set 
            { 
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        private bool CanDeleteImage(object arg)
        {
            return SelectedImage != null;
        }

        private void DeleteImage(object obj)
        {
            _databaseService.ImagesRepository.DeleteImage(SelectedImage.Id);
            _databaseService.SaveChanges();
        }

        private bool CanViewImage(object arg)
        {
            return SelectedImage != null;
        }

        private void ViewImage(object obj)
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
            OnPropertyChanged(nameof(Images));
        }

    }


}
