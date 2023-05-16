using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Interfaces;
using XRayApp.Data.Models;

namespace XRayApp.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DatabaseManager _databaseManager;

        public PatientRepository(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public Patient GetPatientById(int patientId)
        {
            return _databaseManager.Patients.FirstOrDefault(p => p.Id == patientId);
        }

        public void AddPatient(Patient patient)
        {
            _databaseManager.Patients.Add(patient);
            _databaseManager.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _databaseManager.Patients.Update(patient);
            _databaseManager.SaveChanges();
        }

        public void DeletePatient(int patientId)
        {
            var patient = _databaseManager.Patients.FirstOrDefault(p => p.Id == patientId);
            if (patient != null)
            {
                _databaseManager.Patients.Remove(patient);
                _databaseManager.SaveChanges();
            }
        }        
        
        List<Patient> IPatientRepository.GetAll()
        {
            return _databaseManager.Patients.ToList();
        }
    }
}
