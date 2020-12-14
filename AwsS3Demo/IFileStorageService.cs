using System;
using System.IO;
using System.Threading.Tasks;

namespace AwsS3Demo
{
    public interface IFileStorageService : IDisposable
    {
        Task DeleteFileAsync(string key);
        Task GetFileAsync(string key, DirectoryInfo destinationFolder);
        Task<string> StoreFileAsync(FileInfo file);
    }
}