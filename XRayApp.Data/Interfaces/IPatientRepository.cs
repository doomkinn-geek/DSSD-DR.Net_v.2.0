using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Models;

namespace XRayApp.Data.Interfaces
{
    public interface IPatientRepository
    {
        Patient GetPatientById(int patientId);
        void AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        void DeletePatient(int patientId);
        List<Patient> GetAll();
    }
}
