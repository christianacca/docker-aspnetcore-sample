using System;
using Marvin.IDP.Helpers;

namespace Marvin.IDP.Settings
{
    public class GalleryOidcClientSettings
    {
        public string BaseUrl { get; set; }

        public string PostLogoutRedirectUrl => new Uri(BaseUrl).AppendPath("signout-callback-oidc").ToString();

        public string RedirectUrl => new Uri(BaseUrl).AppendPath("signin-oidc").ToString();
    }
}