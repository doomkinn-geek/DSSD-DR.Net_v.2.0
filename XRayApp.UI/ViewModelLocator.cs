using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data;
using XRayApp.UI.ViewModel;

namespace XRayApp.UI
{
    public class ViewModelLocator
    {
        public PatientViewModel PatientViewModel { get; private set; }
        public StudyViewModel StudyViewModel { get; private set; }
        public ImageViewModel ImageViewModel { get; private set; }
        public MainWindowViewModel MainWindowViewModel => new MainWindowViewModel(this);

        public ViewModelLocator(DatabaseManager databaseManager)
        {
            PatientViewModel = new PatientViewModel(databaseManager);
            StudyViewModel = new StudyViewModel(databaseManager);
            ImageViewModel = new ImageViewModel(databaseManager, StudyViewModel);

            PatientViewModel.SelectedPatientChanged += StudyViewModel.OnSelectedPatientChanged;
            StudyViewModel.SelectedStudyChanged += ImageViewModel.LoadImages;
        }
        
    }
}
