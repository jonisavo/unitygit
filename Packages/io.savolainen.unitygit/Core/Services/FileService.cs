using System.IO;

namespace UnityGit.Core.Services
{
    internal interface IFileService
    {
        void Delete(string path);
    }
    
    internal sealed class FileService : IFileService
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
