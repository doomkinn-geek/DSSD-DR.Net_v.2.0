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
using XRayApp.CrossUI.ViewModels;
using XRayApp.Data;
using XRayApp.Data.Models;

namespace XRayApp.CrossUI.ViewModels
{
    public class StudyViewModel : ReactiveObject
    {
        public event Action<Study> SelectedStudyChanged;

        private ObservableCollection<Study> _studies;
        public ObservableCollection<Study> Studies
        {
            get { return _studies; }
            set { this.RaiseAndSetIfChanged(ref _studies, value); }
        }

        private Study _selectedStudy;
        public Study SelectedStudy
        {
            get { return _selectedStudy; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedStudy, value);
                SelectedStudyChanged?.Invoke(_selectedStudy);
            }
        }

        private Patient _selectedPatient;
        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPatient, value);                
            }
        }

        public ImageViewModel ImageViewModel { get; private set; }

        public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; private set; }
        private readonly DatabaseManager _databaseService;

        public StudyViewModel(DatabaseManager databaseService)
        {
            _databaseService = databaseService;
            SaveChangesCommand = ReactiveCommand.Create(SaveChanges, this.WhenAnyValue(x => x.SelectedStudy).Select(study => CanSaveChanges()));
        }

        private bool CanSaveChanges()
        {
            if (SelectedStudy == null) { return false; }
            if (string.IsNullOrWhiteSpace(SelectedStudy.StudyId) || string.IsNullOrWhiteSpace(SelectedStudy.Description))
            {
                return false;
            }
            return true;
        }

        private void SaveChanges()
        {
            _databaseService.StudiesRepository.UpdateStudy(SelectedStudy);
            _databaseService.SaveChanges();
        }

        public void OnSelectedPatientChanged(Patient patient)
        {
            SelectedPatient = patient;
            LoadStudies(patient.Id);
        }

        public void LoadStudies(int patientId)
        {
            Studies = new ObservableCollection<Study>(_databaseService.StudiesRepository.GetStudiesByPatientId(patientId));
            if (Studies.Any())
            {
                SelectedStudy = Studies.First();
            }
            else
            {
                SelectedStudy = null;
            }
        }
    }


}
