using System.IO;
using System.Text;
using LibGit2Sharp;
using UnityEditor;
using UnityEngine;

namespace UnityGit.GUI.Services
{
    public class DiffService : IDiffService
    {
        public void DiffFile(IRepository repository, string filePath)
        {
            // var patch = repository.Diff.Compare<Patch>(new[] { filePath });

            var headFilePath = Path.Combine(Application.temporaryCachePath, "HEAD");
            
            var blob = repository.Head.Tip[filePath].Target as Blob;

            if (blob == null)
                return;
            
            using (var content = new StreamReader(blob.GetContentStream(), Encoding.UTF8))
            {
                File.WriteAllText(headFilePath, content.ReadToEnd());
            }

            var currentFilePath = Path.Combine(repository.Info.WorkingDirectory, filePath);

            EditorUtility.InvokeDiffTool(
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