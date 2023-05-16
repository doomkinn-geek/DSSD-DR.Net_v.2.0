using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Models;

namespace XRayApp.Data.Interfaces
{
    public interface IStudyRepository
    {
        Study GetStudyById(int studyId);
        void AddStudy(Study study);
        void UpdateStudy(Study study);
        void DeleteStudy(int studyId);
        IEnumerable<Study> GetStudiesByPatientId(int id);
    }
}
