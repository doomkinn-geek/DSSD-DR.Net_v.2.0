using XRayApp.Data.Models;
using XRayApp.Data;
using XRayApp.CrossUI.ViewModels;
using ReactiveUI;

namespace XRayApp.CrossUI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private DatabaseManager _databaseService;
        private PatientViewModel _patientViewModel;
        private StudyViewModel _studyViewModel;
        private ImageViewModel _imageViewModel;
        private ViewModelLocator _locator;

        public PatientViewModel PatientViewModel => _locator.PatientViewModel;
        public StudyViewModel StudyViewModel => _locator.StudyViewModel;
        public ImageViewModel ImageViewModel => _locator.ImageViewModel;

        public MainWindowViewModel(ViewModelLocator locator)
        {
            _locator = locator;
        }

        private void OnSelectedPatientChanged(Patient patient)
        {
            // Здесь мы предполагаем, что у вас есть метод LoadStudies в StudyViewModel
            StudyViewModel.LoadStudies(patient.Id);
        }

        private void OnSelectedStudyChanged(Study study)
        {
            // Здесь мы предполагаем, что у вас есть метод LoadImages в ImageViewModel
            ImageViewModel.LoadImages(study);
        }
    }
}