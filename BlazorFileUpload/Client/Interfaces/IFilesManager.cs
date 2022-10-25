using BlazorFileUpload.Shared;

namespace BlazorFileUpload.Client.Interfaces
{
    /// <summary>
    /// IFileManager
    /// </summary>
    public interface IFilesManager
    {
        /// <summary>
        /// Gets the file names.
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetFileNames();

        /// <summary>
        /// Uploads the file chunk.
        /// </summary>
        /// <param name="fileChunkDto">The file chunk dto.</param>
        /// <returns></returns>
        Task<bool> UploadFileChunk(FileChunk fileChunkDto);
    }
}