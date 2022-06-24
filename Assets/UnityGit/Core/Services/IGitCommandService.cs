using System.Diagnostics;
using UnityGit.UnityGit.Core.Data;

namespace UnityGit.Core.Services
{
    public interface IGitCommandService
    {
        Process Run(GitCommandInfo info);
        
        bool IsRunning { get; }
    }
}