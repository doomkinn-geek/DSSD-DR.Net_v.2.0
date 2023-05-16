using FellowOakDicom.Network.Client;
using FellowOakDicom.Network;
using FellowOakDicom;
using System;
using System.Threading.Tasks;

namespace XRayApp.Core
{
    public class DicomSender
    {
        private readonly string _remoteHost;
        private readonly int _remotePort;
        private readonly string _remoteAET;
        private readonly string _localAET;

        public DicomSender(string remoteHost, int remotePort, string remoteAET, string localAET)
        {
            _remoteHost = remoteHost;
            _remotePort = remotePort;
            _remoteAET = remoteAET;
            _localAET = localAET;
        }

        public async Task SendAsync(XRayImage image, DicomDataset patientData)
        {
            // Создаем новый DICOM-файл с базовой информацией
            DicomFile dicomFile = new DicomFile();
            dicomFile.Dataset.Add(DicomTag.PatientID, patientData.GetValue<string>(DicomTag.PatientID, 0));
            dicomFile.Dataset.Add(DicomTag.PatientName, patientData.GetValue<string>(DicomTag.PatientName, 0));
            dicomFile.Dataset.Add(DicomTag.StudyInstanceUID, image.DicomStudyUID);
            dicomFile.Dataset.Add(DicomTag.SeriesInstanceUID, image.DicomSeriesUID);

            // Заполняем DICOM-файл данными изображения
            byte[] pixelData = ConvertToByteArray(image.ImageData);
            dicomFile.Dataset.Add(DicomTag.PixelData, pixelData);

            // Сохраняем DICOM-файл
            dicomFile.Save("temp.dcm");

            // Создаем клиент для отправки DICOM-файла
            // Отправляем DICOM-файл на удаленный узел
            var client = DicomClientFactory.Create(_remoteHost, _remotePort, false, _localAET, _remoteAET);
            await client.AddRequestAsync(new DicomCStoreRequest("temp.dcm"));
            await client.SendAsync();
        }

        private byte[] ConvertToByteArray(ushort[] imageData)
        {
            byte[] byteArray = new byte[imageData.Length * sizeof(ushort)];
            Buffer.BlockCopy(imageData, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }

}
