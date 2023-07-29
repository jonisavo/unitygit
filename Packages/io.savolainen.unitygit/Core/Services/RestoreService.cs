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
        
        bool RestoreFile(IRepository repository, string filePath);
    }
    
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    [Dependency(typeof(ICheckoutService), provide: typeof(CheckoutService))]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    [Dependency(typeof(IFileService), provide: typeof(FileService))]
    public sealed partial class RestoreService : Service, IRestoreService
    {
        public event IRestoreService.FileRestoredDelegate FileRestored;

        [Provide]
        private IDialogService _dialogService;
        [Provide]
        private ICheckoutService _checkoutService;
        [Provide]
        private ILogService _logService;
        [Provide]
        private IFileService _fileService;

        public bool RestoreFile(IRepository repository, string filePath)
        {
            var status = repository.RetrieveStatus(filePath);

            if (status == FileStatus.Nonexistent)
                return false;
            
            if (FileStatusUtilities.IsNew(status))
            {
                if (!TryDeleteFile(repository, filePath))
                    return false;
            }
            else
            {
                if (!TryRestoreFile(repository, filePath))
                    return false;
            }

            FileRestored?.Invoke(repository, filePath);

            return true;
        }

        public bool TryDeleteFile(IRepository repository, string filePath)
        {
            if (!_dialogService.Confirm($"Are you sure you want to delete {filePath}?"))
                return false;

            var success = false;
            
            var path = Path.Combine(repository.Info.WorkingDirectory, filePath);
            _logService.LogMessage($"Deleting file {path}...");
            
            try
            {
                _fileService.Delete(path);
                success = true;
                _logService.LogMessage("File deleted.");
            } catch (Exception exception)
            {
                _dialogService.Error($"Failed to delete {path}.");
                _logService.LogException(exception);
            }

            return success;
        }

        public bool TryRestoreFile(IRepository repository, string filePath)
        {
            if (!_dialogService.Confirm($"Are you sure you want to restore {filePath}?"))
                return false;
                
            var success = false;
            
            var path = Path.Combine(repository.Info.WorkingDirectory, filePath);
            _logService.LogMessage($"Restoring file {path}...");

            try
            {
                _checkoutService.ForceCheckoutPaths(repository, new[] {filePath});
                success = true;
                _logService.LogMessage("File restored.");
            }
            catch (Exception exception)
            {
                _dialogService.Error($"Failed to restore {path}.");
                _logService.LogException(exception);
            }

            return success;
        }
    }
}
