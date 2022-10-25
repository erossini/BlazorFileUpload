namespace BlazorFileUpload.Shared
{
    /// <summary>
    /// File Chunk
    /// </summary>
    public class FileChunk
    {
        /// <summary>
        /// The first chunk
        /// </summary>
        public bool FirstChunk = false;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[]? Data { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public long Offset { get; set; }
    }
}