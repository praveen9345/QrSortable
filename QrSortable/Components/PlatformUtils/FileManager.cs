namespace QrSortable.Components.PlatformUtils
{
   using Wrappers;

    /// <summary>
    ///     Implementation of the file manager interface that executes file operations.
    /// </summary>
    public class FileManager : IFileManager
    {
        private readonly IFileWrapper _fileWrapper;
        private readonly IMauiEssentialsWrapper _mauiEssentialsWrapper;

        /// <summary>
        ///     Initializes an instance of the <see cref="FileManager" /> class.
        /// </summary>
        /// <param name="mauiEssentialsWrapper">The maui wrapper for identification of the current platform.</param>
        /// <param name="fileWrapper">The file wrapper for executing system file operations.</param>
        public FileManager(IMauiEssentialsWrapper mauiEssentialsWrapper, IFileWrapper fileWrapper)
        {
            _mauiEssentialsWrapper = mauiEssentialsWrapper;
            _fileWrapper = fileWrapper;
        }

        
        /// <summary>
        ///     Stores a resource in the private system folder. Replaces old files.
        /// </summary>
        /// <param name="fileName">The name of the file to store.</param>
        /// <param name="file">The file to store.</param>
        /// <returns>If writing the file was successful or not.</returns>
        public async Task<bool> WriteFileToFileSystemAsync(string fileName, byte[] file)
        {
            var fullFilePath = GetFullPrivateSystemFilePath(fileName);
            // if file already exists in private system folder, delete it. So that if a new file has been locally stored
            // in resources, this new file is being used.
            if (_fileWrapper.DoesFileExist(fullFilePath))
            {
                /* if (!await _fileWrapper.DeleteFileAsync(fullFilePath))
                {
                    return false;
                } */

                return false;
            }

            var wasCreatingFileSuccessful = await _fileWrapper.CreateAndWriteToFileAsync(fullFilePath, file);
            return wasCreatingFileSuccessful;
        }

        /// <summary>
        ///     Opens the file with the given filename from the private system folder.
        /// </summary>
        /// <param name="fileName">The name of the file which shall be opened.</param>
        /// <returns>A bool indicating whether the opening of the file was successful.</returns>
        public async Task<bool> OpenStoredFileFromSystemFolderAsync(string fileName)
        {
            var fullFilePath = GetFullPrivateSystemFilePath(fileName);

            if (!_fileWrapper.DoesFileExist(fullFilePath))
            {
                return false;
            }

            await _mauiEssentialsWrapper.OpenFileAsync(fullFilePath);

            return true;
        }


        /// <summary>
        ///     Gets the stram of the resources file.
        /// </summary>
        /// <param name="filePath"> The path of the resources file. </param>
        /// <returns> The stream of the passed file. </returns>
        ///e.g filepath= "Resources.Embedded.Product.xlsx"
        public Stream GetStreamOfResourcesFile(string filePath)
         {
            var stream = _fileWrapper.GetResourceStream(filePath);

            if (stream == null) return null;

            return stream;
         }

        /// <summary>
        ///     Gets the path string of the file.
        /// </summary>
        /// <param name="fileName"> The name of the file to check. </param>
        /// <returns> The path string of the passed file. </returns>
        public string GetFullPrivateSystemFilePath(string fileName)
        {
            
            var currentPlatform = _mauiEssentialsWrapper.GetDevicePlatform();
            if (currentPlatform == _mauiEssentialsWrapper.AndroidDevicePlatform || currentPlatform ==_mauiEssentialsWrapper.WindowsDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fileName);
            }

            if (currentPlatform == _mauiEssentialsWrapper.IosDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Library", fileName);
            }

            throw new NotImplementedException("The current platform is not supported");
        }

    }
}