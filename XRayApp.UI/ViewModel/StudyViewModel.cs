using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XRayApp.Data;
using XRayApp.Data.Models;
using XRayApp.UI.Commands;

namespace XRayApp.UI.ViewModel
{
    public class StudyViewModel : BaseViewModel
    {
        public event Action<Study> SelectedStudyChanged;



        private ObservableCollection<Study> _studies;
        public ObservableCollection<Study> Studies
        {
            get { return _studies; }
            set
            {
                _studies = value;
                OnPropertyChanged();
            }
        }

        private Study _selectedStudy;
        public Study SelectedStudy
        {
            get { return _selectedStudy; }
            set
            {
                _selectedStudy = value;
                OnPropertyChanged();
                SelectedStudyChanged?.Invoke(_selectedStudy);
            }
        }


        public ImageViewModel ImageViewModel { get; private set; }


        public ICommand SaveChangesCommand { get; private set; }
        private readonly DatabaseManager _databaseService;

        public StudyViewModel(DatabaseManager databaseService)
        {
            _databaseService = databaseService;            
            SaveChangesCommand = new RelayCommand(param => SaveChanges(), param => CanSaveChanges());
        }

        private bool CanSaveChanges()
        {
            if(SelectedStudy == null) { return false; }
            // Проверяем, что обязательные поля не пусты
            if (string.IsNullOrWhiteSpace(SelectedStudy.StudyId) || string.IsNullOrWhiteSpace(SelectedStudy.Description))
            {
                return false;
            }

            // Если обязательные поля заполнены, то изменения можно сохранять
            return true;
        }


        private void SaveChanges()
        {
            // Вызываем метод обновления из сервиса базы данных
            _databaseService.StudiesRepository.UpdateStudy(SelectedStudy);

            // Сохраняем изменения в базе данных
            _databaseService.SaveChanges();
        }

        public void OnSelectedPatientChanged(Patient patient)
        {
            // Здесь мы предполагаем, что у вас есть метод LoadStudies
            LoadStudies(patient.Id);
        }

        public void LoadStudies(int patientId)
        {
            // Используя ваш сервис для работы с базой данных, загружаем обследования для конкретного пациента
            Studies = new ObservableCollection<Study>(_databaseService.StudiesRepository.GetStudiesByPatientId(patientId));

            // Если есть обследования, устанавливаем первое из них как выбранное
            if (Studies.Any())
            {
                SelectedStudy = Studies.First();
            }
            else
            {
                SelectedStudy = null;
            }

            OnPropertyChanged(nameof(Studies));
        }


    }

}
