using System.Threading.Tasks;
using UnityGit.Core.Data;
using UnityGit.UnityGit.Core.Data;

namespace UnityGit.Core.Services
{
    public interface IGitCommandService
    {
        Task<GitProcessResult> Run(GitCommandInfo info);
        
        bool IsRunning { get; }
    }
}