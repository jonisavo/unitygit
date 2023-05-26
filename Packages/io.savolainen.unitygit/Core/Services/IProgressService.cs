namespace UnityGit.Core.Services
{
    public struct ProgressOptions
    {
        public bool Sticky;
        public bool Synchronous;
    }
    
    public interface IProgressService
    {
        public int Start(string name, string description, ProgressOptions options);

        public void FinishWithSuccess(int progressId);

        public void FinishWithError(int progressId, string message);
    }
}
