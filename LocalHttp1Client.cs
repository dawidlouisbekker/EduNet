using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Maui.app1
{
    internal class OllamaClienthttp1
    {
        private HttpClient? _httpClient;

        


            public OllamaClienthttp1()
        {
            // No custom handler is required for HTTP/1.1, HttpClient defaults to HTTP/1.1
            _httpClient = new HttpClient
            {
                DefaultRequestVersion = new Version(1, 1),
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower // Ensure it's HTTP/1.1 or lower
            };
        }

        //public static async Task PostJson()

        public async Task<string> GetAsync(string url)
        {
            if (_httpClient == null) throw new InvalidOperationException("HttpClient is not initialized.");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task SendPdfFileAsync(string filePath, string url)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                    content.Add(fileContent, "file", Path.GetFileName(filePath));

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("File uploaded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("File upload failed.");
                    }
                }
            }
        }
    }
}

