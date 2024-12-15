namespace QrSortable.Components.PlatformUtils.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    ///     Wrapper interface for .Net file operations.
    /// </summary>
    public interface IFileWrapper
    {
        /// <summary>
        ///     Deletes the file with the specified file path.
        /// </summary>
        /// <param name="filePath">The file path of the file to delete.</param>
        /// <returns>A value indicating whether the deletion succeeded.</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        ///     Determines whether a file with the given file path exists.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns>A value indicating whether a file with the given file path exists.</returns>
        bool DoesFileExist(string filePath);

        /// <summary>
        ///     Copies the given embedded resource to the specified file path of the device.
        /// </summary>
        /// <param name="embeddedResource">The embedded resource.</param>
        /// <param name="filePath">The file path to copy to.</param>
        /// <returns>A value indicating whether the copying succeeded.</returns>
        Task<bool> CopyEmbeddedResourceToFilePathAsync(string embeddedResource, string filePath);

        /// <summary>
        ///     Downloads and saves the file from the given uri to the specified file path of the device.
        /// </summary>
        /// <param name="url">The link to download the file from.</param>
        /// <param name="filePath">The file path to save the file to.</param>
        /// <returns>A value indicating whether the downloading succeeded.</returns>
        Task<bool> DownloadAndSaveFileToFileSystemAsync(Uri url, string filePath);

        /// <summary>
        ///     Creates a file and writes the passed bytearray to it.
        /// </summary>
        /// <param name="filePath"> The file path where the file should be created. </param>
        /// <param name="bytes"> The bytes to write. </param>
        /// <returns> True, if creating the file and writing to it was successful. False, otherwise.</returns>
        Task<bool> CreateAndWriteToFileAsync(string filePath, byte[] bytes);

        /// <summary>
        ///     Gets all the files in a given directory whose filename contains a certain string.
        /// </summary>
        /// <param name="partOfFileName"> The string to search for in the file names. </param>
        /// <param name="directoryPath"> The path of the directory to search in. </param>
        /// <returns> A list of files in the given directory whose filename contains the given string. </returns>
        List<string> GetFilesInDirectory(string partOfFileName, string directoryPath);

        /// <summary>
        ///     Reads out a binary file and returns a byte array with its contents.
        /// </summary>
        /// <param name="filePath"> The path of the file to read out. </param>
        /// <returns> A byte array with the contents of the file if no exception occurred. Null, otherwise. </returns>
        Task<byte[]> GetByteArrayFromBinaryFileAsync(string filePath);

        /// <summary>
        ///     Gets a stream from a resource.
        /// </summary>
        /// <param name="filePath"> The file path of the resource. </param>
        /// <returns> A stream of the file's content. </returns>
        Stream GetResourceStream(string filePath);
    }
}