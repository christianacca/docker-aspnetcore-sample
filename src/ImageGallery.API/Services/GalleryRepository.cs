using ImageGallery.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.API.Services
{
    public class GalleryRepository : IGalleryRepository, IDisposable
    {
        GalleryContext _context;

        public GalleryRepository(GalleryContext galleryContext)
        {
            _context = galleryContext;
        }
        public bool ImageExists(Guid id)
        {
            return _context.Images.Any(i => i.Id == id);
        }       

        public async Task<Image> GetImageAsync(Guid id)
        {
            return await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
        }
  
        public async Task<ICollection<Image>> GetImagesAsync(string ownerId)
        {
            return await _context.Images
                .Where(i => i.OwnerId == ownerId)
                .OrderBy(i => i.Title).ToListAsync();
        }

        public bool IsImageOwner(Guid id, string ownerId)
        {
            return _context.Images.Any(i => i.Id == id && i.OwnerId == ownerId);
        }


        public void AddImage(Image image)
        {
            _context.Images.Add(image);
        }

        public void UpdateImage(Image image)
        {
            // no code in this implementation
        }

        public void DeleteImage(Image image)
        {
            _context.Images.Remove(image);

            // Note: in a real-life scenario, the image itself should also 
            // be removed from disk.  We don't do this in this demo
            // scenario, as we refill the DB with image URIs (that require
            // the actual files as well) for demo purposes.
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

            }
        }     
    }
}
