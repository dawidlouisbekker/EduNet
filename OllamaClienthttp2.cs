using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maui.app1
{
    internal class OllamaClienthttp2
    {
        private HttpClient? _httpClient;

        // Constructor
        public OllamaClienthttp2()
        {
            var handler = new SocketsHttpHandler
            {
                SslOptions = new SslClientAuthenticationOptions
                {
                    ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http2 }
                }
            };

            _httpClient = new HttpClient(handler)
            {
                DefaultRequestVersion = new Version(2, 0),
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };
        }

        public async Task<string> GetAsync(string url)
        {
            if (_httpClient == null) throw new InvalidOperationException("HttpClient is not initialized.");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
