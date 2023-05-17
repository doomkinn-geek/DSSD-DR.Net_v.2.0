using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.CrossUI.ViewModels;
using XRayApp.Data;


namespace XRayApp.CrossUI
{
    public class ViewModelLocator
    {
        public PatientViewModel PatientViewModel { get; private set; }
        public StudyViewModel StudyViewModel { get; private set; }
        public ImageViewModel ImageViewModel { get; private set; }        
        public MainWindowViewModel MainWindowViewModel => new MainWindowViewModel(this);

        public ViewModelLocator()
        {
            DatabaseManager _databaseManager = new DatabaseManager();
            _databaseManager.InitializeDatabase();

            PatientViewModel = new PatientViewModel(_databaseManager);
            StudyViewModel = new StudyViewModel(_databaseManager);
            ImageViewModel = new ImageViewModel(_databaseManager, StudyViewModel);

            PatientViewModel.SelectedPatientChanged += StudyViewModel.OnSelectedPatientChanged;
            StudyViewModel.SelectedStudyChanged += ImageViewModel.LoadImages;
        }

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
