namespace QrSortable.Components.PlatformUtils
{
    /// <summary>
    ///     Provides support for executing file operations.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        ///     Gets the path string of the file.
        /// </summary>
        /// <param name="fileName"> The name of the file to check. </param>
        /// <returns> The path string of the passed file. </returns>
        string GetFullPrivateSystemFilePath(string fileName);

        /// <summary>
        ///     Stores a resource in the private system folder. Replaces old files.
        /// </summary>
        /// <param name="fileName">The name of the file to store.</param>
        /// <param name="file">The file to store.</param>
        /// <returns>If writing the file was successful or not.</returns>
        Task<bool> WriteFileToFileSystemAsync(string fileName, byte[] file);

        /// <summary>
        ///     Gets the stram of the resources file.
        /// </summary>
        /// <param name="filePath"> The path of the resources file. </param>
        /// <returns> The stream of the passed file. </returns>
        ///e.g filepath= "Resources.Embedded.Product.xlsx"
        Stream GetStreamOfResourcesFile(string filePath);

        /// <summary>
        ///     Opens the file with the given filename from the private system folder.
        /// </summary>
        /// <param name="fileName">The name of the file which shall be opened.</param>
        /// <returns>A bool indicating whether the opening of the file was successful.</returns>
        Task<bool> OpenStoredFileFromSystemFolderAsync(string fileName);
    }
}
