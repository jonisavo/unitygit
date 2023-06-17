using System.Collections.Generic;

namespace UnityGit.Core.Data
{
    /// <summary>
    /// Result of a Git process run.
    /// </summary>
    public sealed class GitProcessResult
    {
        /// <summary>
        /// Whether the process was started.
        /// </summary>
        public bool Started = false;
        
        /// <summary>
        /// Whether the process timed out.
        /// </summary>
        public bool Timeout = false;
        
        /// <summary>
        /// Process exit code. Null if not set.
        /// </summary>
        public int? ExitCode { get; set; } = null;

        public readonly List<OutputLine> Output = new List<OutputLine>(20);
    }
}
