﻿using ImageGallery.API.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageGallery.API.Services
{
    public interface IGalleryRepository
    {
        Task<ICollection<Image>> GetImagesAsync(string ownerId);
        bool IsImageOwner(Guid id, string ownerId);
        Task<Image> GetImageAsync(Guid id);
        bool ImageExists(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        Task<bool> SaveAsync();
    }
}
