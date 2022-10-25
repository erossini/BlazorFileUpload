using BlazorFileUpload.Client.Interfaces;
using BlazorFileUpload.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorFileUpload.Client.Services
{
    /// <summary>
    /// File Manager
    /// </summary>
    /// <seealso cref="BlazorFileUpload.Client.Interfaces.IFilesManager" />
    public class FilesManager : IFilesManager
    {
        HttpClient _http;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesManager"/> class.
        /// </summary>
        /// <param name="http">The HTTP.</param>
        public FilesManager(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Uploads the file chunk.
        /// </summary>
        /// <param name="fileChunkDto">The file chunk dto.</param>
        /// <returns></returns>
        public async Task<bool> UploadFileChunk(FileChunk fileChunkDto)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("api/Files/UploadFileChunk", fileChunkDto);
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();

                return Convert.ToBoolean(responseBody);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the file names.
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetFileNames()
        {
            try
            {
                var response = await _http.GetAsync("api/Files/GetFiles");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<string>>(responseBody);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}