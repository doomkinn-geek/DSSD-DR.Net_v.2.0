using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XRayApp.Data.Models;
using XRayApp.Data;
using XRayApp.UI.Commands;

namespace XRayApp.UI.ViewModel
{
    public class PatientViewModel : BaseViewModel
    {
        public event Action<Patient> SelectedPatientChanged;

        private DatabaseManager _dbManager;
        private Patient _selectedPatient;
        private Study _selectedStudy;
        private Image _selectedImage;

        public ObservableCollection<Patient> Patients { get; set; }
        public ObservableCollection<Study> Studies { get; set; }
        public ObservableCollection<Image> Images { get; set; }

        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                Studies = new ObservableCollection<Study>(_dbManager.StudiesRepository.GetStudiesByPatientId(_selectedPatient.Id));
                OnPropertyChanged(nameof(SelectedPatient));
                SelectedPatientChanged?.Invoke(_selectedPatient);
            }
        }

        public Study SelectedStudy
        {
            get => _selectedStudy;
            set
            {
                _selectedStudy = value;
                Images = new ObservableCollection<Image>(_dbManager.ImagesRepository.GetImagesByStudyId(_selectedStudy.Id));
                OnPropertyChanged(nameof(SelectedStudy));
            }
        }

        public Image SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        public ICommand AddPatientCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand DeletePatientCommand { get; }



        public PatientViewModel(DatabaseManager dbManager)
        {
            _dbManager = dbManager;

            AddPatientCommand = new RelayCommand(AddPatient);
            EditPatientCommand = new RelayCommand(EditPatient, CanExecutePatientCommand);
            DeletePatientCommand = new RelayCommand(DeletePatient, CanExecutePatientCommand);

            LoadData();
        }

        private void LoadData()
        {
            // Load data from database
            var patients = _dbManager.PatientsRepository.GetAll();
            Patients = new ObservableCollection<Patient>(patients);
        }

        private void AddPatient(object obj)
        {
            // Here you can add logic to add a new patient
            var newPatient = new Patient();
            // TODO: Add logic to fill patient data
            _dbManager.PatientsRepository.AddPatient(newPatient);
            _dbManager.SaveChanges();
            LoadData();
        }

        private void EditPatient(object obj)
        {
            // Here you can add logic to edit a selected patient
            // TODO: Add logic to edit patient data
            _dbManager.PatientsRepository.UpdatePatient(SelectedPatient);
            _dbManager.SaveChanges();
            LoadData();
        }

        private void DeletePatient(object obj)
        {
            // Here you can add logic to delete a selected patient
            _dbManager.PatientsRepository.DeletePatient(SelectedPatient.Id);
            _dbManager.SaveChanges();
            LoadData();
        }

        private bool CanExecutePatientCommand(object arg)
        {
            return SelectedPatient != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
