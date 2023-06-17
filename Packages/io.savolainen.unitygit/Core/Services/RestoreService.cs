using System;
using System.IO;
using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Utilities;

namespace UnityGit.Core.Services
{
    public interface IRestoreService
    {
        delegate void FileRestoredDelegate(IRepository repository, string filePath);

        event FileRestoredDelegate FileRestored;
        
        void RestoreFile(IRepository repository, string filePath);
    }
    
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    [Dependency(typeof(ICheckoutService), provide: typeof(CheckoutService))]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public partial class RestoreService : Service, IRestoreService
    {
        public event IRestoreService.FileRestoredDelegate FileRestored;

        [Provide]
        private IDialogService _dialogService;
        [Provide]
        private ICheckoutService _checkoutService;
        [Provide]
        private ILogService _logService;

        public void RestoreFile(IRepository repository, string filePath)
        {
            var status = repository.RetrieveStatus(filePath);

            if (status == FileStatus.Nonexistent)
                return;
            
            if (FileStatusUtilities.IsNew(status))
            {
                if (!TryDeleteFile(repository, filePath))
                    return;
            }
            else
            {
                if (!TryRestoreFile(repository, filePath))
                    return;
            }

            FileRestored?.Invoke(repository, filePath);
        }

        private bool TryDeleteFile(IRepository repository, string filePath)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete {filePath}?"))
                return false;

            var success = false;
            
            var path = Path.Combine(repository.Info.WorkingDirectory, filePath);
            _logService.LogMessage($"Deleting file {path}...");
            
            try
            {
                File.Delete(path);
                success = true;
                _logService.LogMessage("File deleted.");
            } catch (Exception exception)
            {
                _dialogService.Error($"Failed to delete {filePath}.");
                _logService.LogException(exception);
            }

            return success;
        }

        private bool TryRestoreFile(IRepository repository, string filePath)
        {
            if (!_dialogService.Confirm($"Are you sure you want to restore {filePath}?"))
                return false;
                
            var success = false;
            
            _logService.LogMessage($"Restoring file {filePath}...");

            try
            {
                _checkoutService.ForceCheckoutPaths(repository, new[] {filePath});
                success = true;
                _logService.LogMessage("File restored.");
            }
            catch (Exception exception)
            {
                _dialogService.Error($"Failed to restore file {filePath}.");
                _logService.LogException(exception);
            }

            return success;
        }
    }
}
