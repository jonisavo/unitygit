namespace UnityGit.Core.Services
{
    public interface IDialogService
    {
        bool Confirm(string message);

        bool Error(string message);
    }
}