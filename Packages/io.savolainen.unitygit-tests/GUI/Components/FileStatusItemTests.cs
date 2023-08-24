using LibGit2Sharp;
using NSubstitute;
using NUnit.Framework;
using UIComponents.Testing;
using UnityEditor;
using UnityEngine.UIElements;
using UnityGit.Core.Services;
using UnityGit.GUI.Components;

namespace UnityGit.Tests.GUI.Components
{
    [TestFixture]
    public class FileStatusItemTests
    {
        private ICommitService _commitService;
        private IRestoreService _restoreService;
        private IDiffService _diffService;
        private FileStatusItem _fileStatusItem;
        private TestWindow _testWindow;
        
        [SetUp]
        public void SetUp()
        {
            _commitService = Substitute.For<ICommitService>();
            _restoreService = Substitute.For<IRestoreService>();
            _diffService = Substitute.For<IDiffService>();

            _fileStatusItem = new TestBed<FileStatusItem>()
                .WithSingleton(_commitService)
                .WithSingleton(_restoreService)
                .WithSingleton(_diffService)
                .Instantiate();
            
            _testWindow = EditorWindow.GetWindow<TestWindow>();
            _testWindow.AddComponent(_fileStatusItem);
        }

        [TearDown]
        public void TearDown()
        {
            _testWindow.Close();
        }
        
        [Test]
        public void SetStatusEntry_WhenStatusEntryIsUntracked_SetsStateLabelToUntracked()
        {
            var entry = new StatusEntry("file", FileStatus.NewInWorkdir);
            _fileStatusItem.SetStatusEntry(entry);
            
            var stateLabel = _fileStatusItem.Q<Label>("changed-file-state");
            
            Assert.AreEqual("Untracked", stateLabel.text);
        }
    }
}
