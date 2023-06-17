using NUnit.Framework;
using UnityGit.Core.Data;

namespace UnityGit.Tests.Core.Data
{
    [TestFixture]
    public class GitCommandInfoTests
    {
        [Test]
        public void It_Should_Return_Proper_String_Representation()
        {
            var gitCommandInfo = new GitCommandInfo("pull", "Pulling", "Pulling changes", null);
            Assert.That(gitCommandInfo.ToString(), Is.EqualTo("Pulling changes (git pull)"));
        }
    }
}
