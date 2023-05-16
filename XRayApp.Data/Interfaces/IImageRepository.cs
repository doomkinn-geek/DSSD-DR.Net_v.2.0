using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Models;

namespace XRayApp.Data.Interfaces
{
    public interface IImageRepository
    {
        Image GetImageById(int imageId);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(int imageId);
        IEnumerable<Image> GetImagesByStudyId(int studyId);
        List<Image> GetAll();
    }
}
