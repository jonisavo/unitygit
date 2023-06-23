using NUnit.Framework;
using LibGit2Sharp;
using UnityGit.Core.Utilities;

namespace UnityGit.Tests.Core
{
    [TestFixture]
    public class FileStatusUtilitiesTests
    {
        [Test]
        public void IsModified_ReturnsTrue_WhenStatusHasModifiedFlags()
        {
            const FileStatus status = FileStatus.ModifiedInIndex | FileStatus.ModifiedInWorkdir;
            var result = FileStatusUtilities.IsModified(status);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsModified_ReturnsFalse_WhenStatusDoesNotHaveModifiedFlags()
        {
            var result = FileStatusUtilities.IsModified(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsNew_ReturnsTrue_WhenStatusHasNewFlags()
        {
            const FileStatus status = FileStatus.NewInIndex | FileStatus.NewInWorkdir;
            var result = FileStatusUtilities.IsNew(status);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsNew_ReturnsFalse_WhenStatusDoesNotHaveNewFlags()
        {
            var result = FileStatusUtilities.IsNew(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsRenamed_ReturnsTrue_WhenStatusHasRenamedFlags()
        {
            const FileStatus status = FileStatus.RenamedInIndex | FileStatus.RenamedInWorkdir;
            var result = FileStatusUtilities.IsRenamed(status);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsRenamed_ReturnsFalse_WhenStatusDoesNotHaveRenamedFlags()
        {
            var result = FileStatusUtilities.IsRenamed(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsDeleted_ReturnsTrue_WhenStatusHasDeletedFlags()
        {
            const FileStatus status = FileStatus.DeletedFromIndex | FileStatus.DeletedFromWorkdir;
            var result = FileStatusUtilities.IsDeleted(status);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsDeleted_ReturnsFalse_WhenStatusDoesNotHaveDeletedFlags()
        {
            var result = FileStatusUtilities.IsDeleted(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }

        [Test]
        public void Exists_ReturnsTrue_WhenStatusDoesNotExistFlagsAreNotSet()
        {
            var result = FileStatusUtilities.Exists(FileStatus.Unaltered);

            Assert.IsTrue(result);
        }

        [Test]
        public void Exists_ReturnsFalse_WhenStatusHasNonexistentFlag()
        {
            var result = FileStatusUtilities.Exists(FileStatus.Nonexistent);

            Assert.IsFalse(result);
        }

        [Test]
        public void Exists_ReturnsFalse_WhenStatusHasDeletedFlags()
        {
            const FileStatus status = FileStatus.DeletedFromIndex | FileStatus.DeletedFromWorkdir;
            var result = FileStatusUtilities.Exists(status);
            
            Assert.IsFalse(result);
        }
    }
}
