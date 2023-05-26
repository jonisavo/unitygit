using JetBrains.Annotations;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ISignatureService
    {
        [CanBeNull]
        Signature GetSignature();
    }
}