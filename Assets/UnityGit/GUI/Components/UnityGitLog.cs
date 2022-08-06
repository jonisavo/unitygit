using UIComponents;
using UIComponents.Experimental;
using UnityEngine.UIElements;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("UnityGitLog/UnityGitLog")]
    [Stylesheet("UnityGitLog/UnityGitLog.style")]
    [RootClass("ugit-full-height")]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public class UnityGitLog : UnityGitUIComponent
    {
        public new class UxmlFactory : UxmlFactory<UnityGitLog> {}
        
        [Query("output-scroll-view")]
        private readonly ScrollView _scrollView;
        
        private readonly UnityGitLogService _logService;

        public UnityGitLog()
        {
            _logService = Provide<ILogService>() as UnityGitLogService;

            foreach (var line in _logService.GetOutputLines())
                AddLogLine(line);

            _logService.OutputReceived += AddLogLine;
        }
        
        ~UnityGitLog()
        {
            _logService.OutputReceived -= AddLogLine;
        }

        private void AddLogLine(ILogService.OutputLine line)
        {
            var label = new Label(line.Text);
            
            if (line.IsError)
                label.AddToClassList("error");
            
            _scrollView.Add(label);
        }
    }
}