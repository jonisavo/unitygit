using UnityEditor;
using UnityEngine.TestTools;

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
    
    /// <summary>Acts as a wrapper for Unity's Progress API.</summary>
    [ExcludeFromCoverage]
    public sealed class ProgressService : IProgressService
    {
        public int Start(string name, string description, ProgressOptions options)
        {
            var flags = Progress.Options.Indefinite;

            if (options.Sticky)
                flags |= Progress.Options.Sticky;

            if (options.Synchronous)
                flags |= Progress.Options.Synchronous;
            
            return Progress.Start(name, description, flags);
        }

        public void FinishWithSuccess(int progressId)
        {
            Progress.Report(progressId, 1, 1);
            Progress.Finish(progressId, Progress.Status.Succeeded);
        }

        public void FinishWithError(int progressId, string message)
        {
            Progress.SetDescription(progressId, message);
            Progress.Finish(progressId, Progress.Status.Failed);
        }
    }
}
