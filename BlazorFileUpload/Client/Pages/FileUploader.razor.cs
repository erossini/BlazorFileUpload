using BlazorFileUpload.Client.Interfaces;
using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFileUpload.Client.Pages
{
    public partial class FileUploader
    {
        private string dropClass = string.Empty;
        private string ErrorMessage = string.Empty;

        private List<FileUploadProgress> filesQueue = new();

        private bool isUploading = false;
        private int maxAllowedFiles = 3;

        [Inject] private IFilesManager? FilesManager { get; set; }

        #region Records

        record FileUploadProgress(IBrowserFile File, string FileName, long Size, int FileId)
        {
            public IBrowserFile FileData { get; set; } = File;
            public int FileId { get; set; } = FileId;
            public long UploadedBytes { get; set; }
            public double UploadedPercentage => (double)UploadedBytes / (double)Size * 100d;
            public bool HasBeenUploaded { get; set; } = false;
        }

        #endregion Records

        private void AddFilesToQueue(InputFileChangeEventArgs e)
        {
            dropClass = string.Empty;
            ErrorMessage = string.Empty;

            if (e.FileCount > maxAllowedFiles)
            {
                ErrorMessage = $"A maximum of {maxAllowedFiles} is allowed, you have selected {e.FileCount} files!";
            }
            else
            {
                var files = e.GetMultipleFiles(maxAllowedFiles);
                var fileCount = filesQueue.Count;

                foreach (var file in files)
                {
                    var progress = new FileUploadProgress(file, file.Name, file.Size, fileCount);
                    filesQueue.Add(progress);
                    fileCount++;
                }
            }
        }

        private void ClearFileQueue()
        {
            filesQueue.Clear();
        }

        private void HandleDragEnter()
        {
            dropClass = "dropzone-active";
        }

        private void HandleDragLeave()
        {
            dropClass = string.Empty;
        }

        private void RemoveFromQueue(int fileId)
        {
            var itemToRemove = filesQueue.SingleOrDefault(x => x.FileId == fileId);
            if (itemToRemove != null)
                filesQueue.Remove(itemToRemove);
        }

        private async Task UploadChunks(FileUploadProgress file)
        {
            var TotalBytes = file.Size;
            long chunkSize = 400000;
            long numChunks = TotalBytes / chunkSize;
            long remainder = TotalBytes % chunkSize;

            string nameOnly = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            string newFileNameWithoutPath = $"{DateTime.Now.Ticks}-{nameOnly}{extension}";

            bool firstChunk = true;
            using (var inStream = file.FileData.OpenReadStream(long.MaxValue))
            {
                for (int i = 0; i < numChunks; i++)
                {
                    var buffer = new byte[chunkSize];
                    await inStream.ReadAsync(buffer, 0, buffer.Length);

                    var chunk = new FileChunk
                    {
                        Data = buffer,
                        FileName = newFileNameWithoutPath,
                        Offset = filesQueue[file.FileId].UploadedBytes,
                        FirstChunk = firstChunk
                    };

                    await FilesManager.UploadFileChunk(chunk);
                    firstChunk = false;

                    // Update our progress data and UI
                    filesQueue[file.FileId].UploadedBytes += chunkSize;
                    await InvokeAsync(StateHasChanged);
                }

                if (remainder > 0)
                {
                    var buffer = new byte[remainder];
                    await inStream.ReadAsync(buffer, 0, buffer.Length);

                    var chunk = new FileChunk
                    {
                        Data = buffer,
                        FileName = newFileNameWithoutPath,
                        Offset = filesQueue[file.FileId].UploadedBytes,
                        FirstChunk = firstChunk
                    };
                    await FilesManager.UploadFileChunk(chunk);

                    // Update our progress data and UI
                    filesQueue[file.FileId].UploadedBytes += remainder;

                    // await ListFiles();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        private async Task UploadFileQueue()
        {
            isUploading = true;
            await InvokeAsync(StateHasChanged);

            foreach (var file in filesQueue.OrderByDescending(x => x.FileId))
                if (!file.HasBeenUploaded)
                {
                    await UploadChunks(file);
                    file.HasBeenUploaded = true;
                }

            isUploading = false;
        }
    }
}