namespace QrSortable.Components.PlatformUtils
{
    using Wrappers;
    using System.Reflection;

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
        ///     Opens an embedded File. First that file has to be copied and stored to the private system folder.
        ///     Then the file can be opened from there. If the file already exists in the private system folder
        ///     it will be deleted before copying the embedded resource. This is being done to make sure that
        ///     always the most up to date version of the file being opened.
        /// </summary>
        /// <param name="fileName">The name of the file to open.</param>
        /// <returns>A bool indicating whether the opening of the file was successful.</returns>
        public async Task<bool> OpenEmbeddedFileAsync(string fileName)
        {
            // First the embedded resource has to be stored in the private system folder.
            // Only from there files can be opened.
            var filePath = await StoreResourceInPrivateSystemFolderAsync(fileName);
            if (filePath is null)
            {
                return false;
            }

            await _mauiEssentialsWrapper.OpenFileAsync(filePath);

            return true;
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
        ///     Stores a resource in the private system folder. Replaces old files.
        /// </summary>
        /// <param name="fileName">The name of the file to store.</param>
        /// <returns>The full file path of the file.</returns>
        public async Task<string> StoreResourceInPrivateSystemFolderAsync(string fileName)
        {
            var fullFilePath = GetFullPrivateSystemFilePath(fileName);
            // if file already exists in private system folder, delete it. So if a new file has been locally stored
            // in resources, this new file is being used.
            if (_fileWrapper.DoesFileExist(fullFilePath))
            {
                if (!await _fileWrapper.DeleteFileAsync(fullFilePath))
                {
                    return null;
                }
            }

            if (!await SaveEmbeddedFileToFileSystemAsync(fileName, fullFilePath))
            {
                return null;
            }

            return fullFilePath;
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
                if (!await _fileWrapper.DeleteFileAsync(fullFilePath))
                {
                    return false;
                }
            }

            var wasCreatingFileSuccessful = await _fileWrapper.CreateAndWriteToFileAsync(fullFilePath, file);
            return wasCreatingFileSuccessful;
        }


        private async Task<bool> SaveEmbeddedFileToFileSystemAsync(string fileName, string filePath)
        {
            var embeddedResource = GetFullFileNameOfEmbeddedResource(fileName);
            return await _fileWrapper.CopyEmbeddedResourceToFilePathAsync(embeddedResource, filePath);
        }

        private string GetFullFileNameOfEmbeddedResource(string fileName)
        {
            if (fileName.EndsWith(".pdf"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            var embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return embeddedResources.FirstOrDefault(er => er.Contains(fileName));
        }

        private string GetFullPrivateSystemFilePath(string fileName)
        {

            var currentPlatform = _mauiEssentialsWrapper.GetDevicePlatform();
            if (currentPlatform == _mauiEssentialsWrapper.AndroidDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), fileName);
            }

            if (_mauiEssentialsWrapper.GetDevicePlatform() == _mauiEssentialsWrapper.IosDevicePlatform)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", fileName);
            }

            throw new NotImplementedException("The current platform is not supported");
        }
    }
}