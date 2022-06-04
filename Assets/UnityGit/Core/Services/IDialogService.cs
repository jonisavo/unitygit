namespace UnityGit.Core.Services
{
    public interface IDialogService
    {
        public bool Confirm(string message);

        public bool Error(string message);
    }
}