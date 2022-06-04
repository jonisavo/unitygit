using System;
using System.IO;
using LibGit2Sharp;
using UIComponents;
using UnityGit.Core.Utilities;

namespace UnityGit.Core.Services
{
    [Dependency(typeof(IDialogService), provide: typeof(DialogService))]
    [Dependency(typeof(ICheckoutService), provide: typeof(CheckoutService))]
    public class RestoreService : Service, IRestoreService
    {
        public event IRestoreService.FileRestoredDelegate FileRestored;

        private readonly IDialogService _dialogService;
        private readonly ICheckoutService _checkoutService;

        public RestoreService()
        {
            _dialogService = Provide<IDialogService>();
            _checkoutService = Provide<ICheckoutService>();
        }

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

            try
            {
                var path = Path.Combine(repository.Info.WorkingDirectory, filePath);
                File.Delete(path);
                success = true;
            } catch (Exception)
            {
                _dialogService.Error($"Failed to delete {filePath}.");
            }

            return success;
        }

        private bool TryRestoreFile(IRepository repository, string filePath)
        {
            if (!_dialogService.Confirm($"Are you sure you want to restore {filePath}?"))
                return false;
                
            var success = false;

            try
            {
                _checkoutService.ForceCheckoutPaths(repository, new[] {filePath});
                success = true;
            }
            catch (Exception)
            {
                _dialogService.Error($"Failed to restore file {filePath}.");
            }

            return success;
        }
    }
}