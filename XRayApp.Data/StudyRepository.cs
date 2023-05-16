using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Interfaces;
using XRayApp.Data.Models;

namespace XRayApp.Data
{
    public class StudyRepository : IStudyRepository
    {
        private readonly DatabaseManager _databaseManager;

        public StudyRepository(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public Study GetStudyById(int studyId)
        {
            return _databaseManager.Studies.FirstOrDefault(s => s.Id == studyId);
        }

        public void AddStudy(Study study)
        {
            _databaseManager.Studies.Add(study);
            _databaseManager.SaveChanges();
        }

        public void UpdateStudy(Study study)
        {
            _databaseManager.Studies.Update(study);
            _databaseManager.SaveChanges();
        }

        public void DeleteStudy(int studyId)
        {
            var study = _databaseManager.Studies.FirstOrDefault(s => s.Id == studyId);
            if (study != null)
            {
                _databaseManager.Studies.Remove(study);
                _databaseManager.SaveChanges();
            }
        }

        public IEnumerable<Study> GetStudiesByPatientId(int id)
        {
            return _databaseManager.Studies.Where(i => i.PatientId == id).ToList();
        }
    }
}
