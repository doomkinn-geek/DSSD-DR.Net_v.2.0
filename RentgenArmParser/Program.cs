using XRayApp.Data.Models;
using XRayApp.Data;

namespace RentgenArmParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var sourceContext = new SecondDbContext()) // Инициализируйте контекст для второй базы данных
            using (var targetContext = new DatabaseManager()) // Инициализируйте контекст для первой базы данных
            {
                var patients = sourceContext.Patients.ToList();
                var studies = sourceContext.Studies.ToList();
                var images = sourceContext.Pictures.ToList();

                foreach (var patient in patients)
                {
                    // Создание экземпляра пациента в целевой базе данных
                    var targetPatient = new Patient
                    {
                        PatientId = patient.PatientID,
                        FirstName = patient.name,
                        LastName = patient.surname,
                        MiddleName = patient.fathername,
                        BirthDate = patient.birthday,
                        Gender = patient.sex,
                        Address = patient.address,
                        Comment = patient.comment
                    };

                    targetContext.Patients.Add(targetPatient);
                    targetContext.SaveChanges();

                    foreach (var study in studies.Where(s => s.id_patient == patient.id_patient))
                    {
                        // Создание экземпляра исследования в целевой базе данных
                        var targetStudy = new Study
                        {
                            StudyId = study.StudyID,
                            StudyDate = study.date_study ?? DateTime.MinValue,
                            Description = study.StudyDiscr,
                            PatientId = targetPatient.Id
                        };

                        targetContext.Studies.Add(targetStudy);
                        targetContext.SaveChanges();

                        foreach (var image in images.Where(i => i.id_study == study.id_study))
                        {
                            // Создание экземпляра изображения в целевой базе данных
                            var targetImage = new Image
                            {
                                ImageId = image.id_picture.ToString(),
                                ImageDate = image.date_p,
                                ImagePath = image.file_p,
                                SeriesUID = image.series_UID,
                                StudyUID = study.study_UID,
                                ExposureParameters = $"{image.kv} kV, {image.ma} mA, {image.ms} ms",
                                StudyId = targetStudy.Id // Добавьте эту строку
                            };
                            targetContext.Images.Add(targetImage);
                        }

                        targetContext.SaveChanges();
                    }

                    // Комментарий на каждую итерацию с данными пациента
                    Console.WriteLine($"Перенос данных для пациента {patient.PatientID} завершен.");
                }
            }

            Console.WriteLine("Перенос данных завершен.");

        }

        private static string GetValueOrDefault(object value)
        {
            return value == DBNull.Value ? string.Empty : (string)value;
        }
    }
}