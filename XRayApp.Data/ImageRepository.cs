using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Interfaces;
using XRayApp.Data.Models;

namespace XRayApp.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly DatabaseManager _databaseManager;

        public ImageRepository(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public Image GetImageById(int imageId)
        {
            return _databaseManager.Images.FirstOrDefault(i => i.Id == imageId);
        }

        public void AddImage(Image image)
        {
            _databaseManager.Images.Add(image);
            _databaseManager.SaveChanges();
        }

        public void UpdateImage(Image image)
        {
            _databaseManager.Images.Update(image);
            _databaseManager.SaveChanges();
        }

        public void DeleteImage(int imageId)
        {
            var image = _databaseManager.Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
            {
                _databaseManager.Images.Remove(image);
                _databaseManager.SaveChanges();
            }
        }

        public IEnumerable<Image> GetImagesByStudyId(int studyId)
        {
            return _databaseManager.Images.Where(i => i.StudyId == studyId).ToList();
        }
        public List<Image> GetAll()
        {
            return _databaseManager.Images.ToList();
        }

    }
}
