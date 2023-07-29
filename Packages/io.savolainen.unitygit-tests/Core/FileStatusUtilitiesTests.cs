using NUnit.Framework;
using LibGit2Sharp;
using NSubstitute;
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
        public void IsModified_ReturnsTrue_WhenEntryHasModifiedFlags()
        {
            var entry = Substitute.For<StatusEntry>();
            entry.State.Returns(FileStatus.ModifiedInIndex | FileStatus.ModifiedInWorkdir);
            
            var result = FileStatusUtilities.IsModified(entry);
            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsModified_ReturnsFalse_WhenStatusDoesNotHaveModifiedFlags()
        {
            var result = FileStatusUtilities.IsModified(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsModified_ReturnsFalse_WhenEntryDoesNotHaveModifiedFlags()
        {
            var entry = Substitute.For<StatusEntry>();
            entry.State.Returns(FileStatus.Unaltered);
            
            var result = FileStatusUtilities.IsModified(entry);
            
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
        public void IsNew_ReturnsTrue_WhenEntryHasNewFlags()
        {
            var entry = Substitute.For<StatusEntry>();
            entry.State.Returns(FileStatus.NewInIndex | FileStatus.NewInWorkdir);
            
            var result = FileStatusUtilities.IsNew(entry);
            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsNew_ReturnsFalse_WhenStatusDoesNotHaveNewFlags()
        {
            var result = FileStatusUtilities.IsNew(FileStatus.Unaltered);

            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsNew_ReturnsFalse_WhenEntryDoesNotHaveNewFlags()
        {
            var entry = Substitute.For<StatusEntry>();
            entry.State.Returns(FileStatus.Unaltered);
            
            var result = FileStatusUtilities.IsNew(entry);
            
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
        public void Exists_ReturnsTrue_WhenEntryDoesNotExistFlagsAreNotSet()
        {
            var entry = Substitute.For<StatusEntry>();
            entry.State.Returns(FileStatus.Unaltered);
            
            var result = FileStatusUtilities.Exists(entry);
            
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
        
        [Test]
        public void Exists_ReturnsFalse_WhenEntryHasNonexistentOrDeletedFlags()
        {
            var entryDeleted = Substitute.For<StatusEntry>();
            entryDeleted.State.Returns(FileStatus.DeletedFromIndex | FileStatus.DeletedFromWorkdir);
            var entryNonexistent = Substitute.For<StatusEntry>();
            entryNonexistent.State.Returns(FileStatus.Nonexistent);
            
            var deletedResult = FileStatusUtilities.Exists(entryDeleted);
            var nonexistentResult = FileStatusUtilities.Exists(entryNonexistent);
            
            Assert.IsFalse(deletedResult);
            Assert.IsFalse(nonexistentResult);
        }
    }
}
