using System;
using ImageGallery.Model;
using System.Collections.Generic;
using ImageGallery.Client.Helpers;

namespace ImageGallery.Client.ViewModels
{
    public class GalleryIndexViewModel
    {
        public IList<Image> Images { get; set; }
            = new List<Image>();

        public Uri ImageBaseUrl { get; set; }

        public string GetImageUrl(Image item)
        {
            return ImageBaseUrl.AppendPath(item.FileName).ToString();
        }
    }
}
