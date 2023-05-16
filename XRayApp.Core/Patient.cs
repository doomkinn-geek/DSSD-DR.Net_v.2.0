using System;
using System.Collections.Generic;

namespace XRayApp.Core
{
    public class Patient
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PatientID { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }

        public List<Examination> Examinations { get; set; } = new List<Examination>();
    }

}
