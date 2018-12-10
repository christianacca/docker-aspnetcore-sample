using System;
using System.Threading.Tasks;
using Marvin.IDP;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ImageGallery.IntegrationTests
{
    public class TokenProvider_Should: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public TokenProvider_Should(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Expose_Oidc_discovery_endpoint()
        {
            // given
            var client = _factory.CreateClient();

            // when
            var response = await client.GetAsync("/.well-known/openid-configuration");

            // then
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=UTF-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
