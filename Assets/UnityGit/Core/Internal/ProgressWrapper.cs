using UnityEditor;

namespace UnityGit.Core.Internal
{
    internal static class ProgressWrapper
    {
        public static int Start(string name, string description)
        {
#if UNITY_2020_3_OR_NEWER
            return Progress.Start(name, description,
                Progress.Options.Indefinite | Progress.Options.Synchronous);
#else
            return -1;
#endif
        }

        public static void FinishWithSuccess(int progressId)
        {
#if UNITY_2020_3_OR_NEWER
            Progress.Report(progressId, 1, 1);
            Progress.Finish(progressId, Progress.Status.Succeeded);
#endif
        }

        public static void FinishWithError(int progressId, string message)
        {
#if UNITY_2020_3_OR_NEWER
            Progress.SetDescription(progressId, message);
            Progress.Finish(progressId, Progress.Status.Failed);
#endif
        }
    }
}