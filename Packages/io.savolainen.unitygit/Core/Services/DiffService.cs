using System.IO;
using System.Text;
using JetBrains.Annotations;
using LibGit2Sharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityGit.Core.Services
{
    public interface IDiffService
    {
        void DiffFile(IRepository repository, string filePath);
    }
    
    public sealed class DiffService : IDiffService
    {
        public delegate string InvokeDiffToolDelegate(
            string leftTitle,
            string leftFile,
            string rightTitle,
            string rightFile,
            string ancestorTitle,
            string ancestorFile
        );
        
        private readonly InvokeDiffToolDelegate _invokeDiffDelegate;
        
        [PublicAPI] [ExcludeFromCoverage]
        public DiffService() : this(EditorUtility.InvokeDiffTool) {}
        
        public DiffService(InvokeDiffToolDelegate invokeDiffDelegate)
        {
            _invokeDiffDelegate = invokeDiffDelegate;
        }
        
        public void DiffFile(IRepository repository, string filePath)
        {
            var headFilePath = Path.Combine(Application.temporaryCachePath, "HEAD");
            
            var blob = repository.Head.Tip[filePath].Target as Blob;

            if (blob == null)
                return;
            
            using (var content = new StreamReader(blob.GetContentStream(), Encoding.UTF8))
            {
                File.WriteAllText(headFilePath, content.ReadToEnd());
            }

            var currentFilePath = Path.Combine(repository.Info.WorkingDirectory, filePath);

            _invokeDiffDelegate(
                "HEAD",
                headFilePath,
                "Current",
                currentFilePath,
                null,
                null
            );
        }
    }
}
