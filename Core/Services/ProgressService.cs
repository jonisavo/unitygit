using UnityEditor;

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
    
    public class ProgressService : IProgressService
    {
        public int Start(string name, string description, ProgressOptions options)
        {
#if UNITY_2020_3_OR_NEWER
            var flags = Progress.Options.Indefinite;

            if (options.Sticky)
                flags |= Progress.Options.Sticky;

            if (options.Synchronous)
                flags |= Progress.Options.Synchronous;
            
            return Progress.Start(name, description, flags);
#else
            return -1;
#endif
        }

        public void FinishWithSuccess(int progressId)
        {
#if UNITY_2020_3_OR_NEWER
            Progress.Report(progressId, 1, 1);
            Progress.Finish(progressId, Progress.Status.Succeeded);
#endif
        }

        public void FinishWithError(int progressId, string message)
        {
#if UNITY_2020_3_OR_NEWER
            Progress.SetDescription(progressId, message);
            Progress.Finish(progressId, Progress.Status.Failed);
#endif
        }
    }
}
