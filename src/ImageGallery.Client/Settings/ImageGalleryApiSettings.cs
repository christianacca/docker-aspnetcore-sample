using ImageGallery.Client.Helpers;

namespace ImageGallery.Client.Settings
{
    public class ImageGalleryApiSettings
    {
        private string _publicBaseUrl;
        public string BaseUrl { get; set; }

        public string PublicBaseUrl
        {
            get => _publicBaseUrl.IfNullOrWhiteSpace(BaseUrl);
            set => _publicBaseUrl = value;
        }
    }
}