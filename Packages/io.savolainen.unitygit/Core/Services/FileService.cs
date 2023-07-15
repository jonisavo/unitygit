using System.IO;
using UnityEngine.TestTools;

namespace UnityGit.Core.Services
{
    internal interface IFileService
    {
        void Delete(string path);
    }
    
    [ExcludeFromCoverage]
    internal sealed class FileService : IFileService
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
