using System.IO;
using LibGit2Sharp;

namespace UnityGit.GUI.Services
{
    public class RestoreService : IRestoreService
    {
        public event IRestoreService.FileRestoredDelegate FileRestored;

        public void RestoreFile(IRepository repository, string filePath)
        {
            var status = repository.RetrieveStatus(filePath);

            if (status == FileStatus.Nonexistent)
                return;
            
            if (status == FileStatus.NewInIndex || status == FileStatus.NewInWorkdir)
            {
                File.Delete(Path.Combine(repository.Info.WorkingDirectory, filePath));
            }
            else
            {
                var options = new CheckoutOptions
                {
                    CheckoutModifiers = CheckoutModifiers.Force
                };
                repository.CheckoutPaths(
                    repository.Head.FriendlyName,
                    new[] { filePath },
                    options
                );
            }

            FileRestored?.Invoke(repository, filePath);
        }
    }
}