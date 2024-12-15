namespace QrSortable.Components.PlatformUtils.Wrappers
{
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    /// <summary>
    ///     The file wrapper implementation for the standard .Net file operations.
    /// </summary>
    public class FileWrapper : IFileWrapper
    {
        /// <summary>
        ///     Deletes the file with the specified file path.
        /// </summary>
        /// <param name="filePath">The file path of the file to delete.</param>
        /// <returns>A value indicating whether the deletion succeeded.</returns>
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether a file with the given file path exists.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns>A value indicating whether a file with the given file path exists.</returns>
        public bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        ///     Copies the given embedded resource to the specified file path of the device.
        /// </summary>
        /// <param name="embeddedResource">The embedded resource.</param>
        /// <param name="filePath">The file path to copy to.</param>
        /// <returns>A value indicating whether the copying succeeded.</returns>
        public async Task<bool> CopyEmbeddedResourceToFilePathAsync(string embeddedResource, string filePath)
        {
            try
            {
                using Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource);
                using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                resource.CopyTo(file);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        ///     Downloads and saves the file from the given uri to the specified file path of the device.
        /// </summary>
        /// <param name="url">The link to download the file from.</param>
        /// <param name="filePath">The file path to save the file to.</param>
        /// <returns>A value indicating whether the downloading succeeded.</returns>
        public async Task<bool> DownloadAndSaveFileToFileSystemAsync(Uri url, string filePath)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent: Other");
                await webClient.DownloadFileTaskAsync(url, filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FileWrapper:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Creates a file and writes the passed bytearray to it.
        /// </summary>
        /// <param name="filePath"> The file path where the file should be created. </param>
        /// <param name="bytes"> The bytes to write. </param>
        /// <returns> True, if creating the file and writing to it was successful. False, otherwise.</returns>
        public async Task<bool> CreateAndWriteToFileAsync(string filePath, byte[] bytes)
        {
            try
            {
                using (var logFileStream = File.Open(filePath, FileMode.Create))
                {
                    await logFileStream.WriteAsync(bytes, 0, bytes.Length);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FileWrapper: " + e.Message);
                return false;
            }
        }

        /// <summary>
        ///     Gets all the files in a given directory whose filename contains a certain string.
        /// </summary>
        /// <param name="partOfFileName"> The string to search for in the file names. </param>
        /// <param name="directoryPath"> The path of the directory to search in. </param>
        /// <returns> A list of files in the given directory whose filename contains the given string. </returns>
        public List<string> GetFilesInDirectory(string partOfFileName, string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath).Where(s => s.Contains(partOfFileName)).ToList();
        }

        /// <summary>
        ///     Reads out a binary file and returns a byte array with its contents.
        /// </summary>
        /// <param name="filePath"> The path of the file to read out. </param>
        /// <returns> A byte array with the contents of the file if no exception occurred. Null, otherwise. </returns>
        public async Task<byte[]> GetByteArrayFromBinaryFileAsync(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("FileWrapper:" + e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Gets a stream from a resource.
        /// </summary>
        /// <param name="filePath"> The file path of the resource. </param>
        ///e.g filepath= "Resources.Embedded.Product.xlsx"
        /// <returns> A stream of the file's content. </returns>
        public Stream GetResourceStream(string filePath)
        {
             var info = Assembly.GetExecutingAssembly().GetName();
             var name = info.Name;
            return Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{name}.{filePath}");
        }
    }
}