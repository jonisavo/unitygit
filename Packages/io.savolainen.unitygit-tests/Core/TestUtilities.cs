using LibGit2Sharp;
using NSubstitute;

namespace UnityGit.Tests.Core
{
    public static class TestUtilities
    {
        public static IRepository CreateMockRepository(string path)
        {
            var repository = Substitute.For<IRepository>();
            var repositoryInfo = Substitute.For<RepositoryInformation>();
            repositoryInfo.WorkingDirectory.Returns(path);
            repository.Info.Returns(repositoryInfo);

            return repository;
        }
    }
}
