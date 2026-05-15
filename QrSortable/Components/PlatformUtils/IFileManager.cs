namespace QrSortable.Components.PlatformUtils
{
    /// <summary>
    ///     Provides support for executing file operations.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        ///     Opens an embedded File. First that file has to be copied and stored to the private system folder.
        ///     Then the file can be opened from there. If the file already exists in the private system folder
        ///     it will be deleted before copying the embedded resource. This is being done to make sure that
        ///     always the most up to date version of the file being opened. 
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>A bool indicating whether the opening of the file was successful.</returns>
        Task<bool> OpenEmbeddedFileAsync(string fileName);

        /// <summary>
        ///     Stores a resource in the private system folder. Replaces old files.
        /// </summary>
        /// <param name="fileName">The name of the file to store.</param>
        /// <param name="file">The file to store.</param>
        /// <returns>If writing the file was successful or not.</returns>
        Task<bool> WriteFileToFileSystemAsync(string fileName, byte[] file);

        /// <summary>
        ///     Opens the file with the given filename from the private system folder.
        /// </summary>
        /// <param name="fileName">The name of the file which shall be opened.</param>
        /// <returns>A bool indicating whether the opening of the file was successful.</returns>
        Task<bool> OpenStoredFileFromSystemFolderAsync(string fileName);

        /// <summary>
        ///     Stores a resource in the private system folder. Replaces old files.
        /// </summary>
        /// <param name="fileName">The name of the file to store.</param>
        /// <returns>The full file path of the file.</returns>
        Task<string> StoreResourceInPrivateSystemFolderAsync(string fileName);


    }
}
