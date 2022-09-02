using UIComponents;
using UnityEngine.UIElements;
using UnityGit.Core.Data;
using UnityGit.Core.Services;

namespace UnityGit.GUI.Components
{
    [Layout("UnityGitLog/UnityGitLog")]
    [Stylesheet("UnityGitLog/UnityGitLog.style")]
    [RootClass("ugit-full-height")]
    [Dependency(typeof(ILogService), provide: typeof(UnityGitLogService))]
    public class UnityGitLog : UnityGitUIComponent, IOnAttachToPanel, IOnDetachFromPanel
    {
        public new class UxmlFactory : UxmlFactory<UnityGitLog> {}
        
        [Query("output-scroll-view")]
        private readonly ScrollView _scrollView;
        
        [Provide(CastFrom = typeof(ILogService))]
        private readonly UnityGitLogService _logService;

        public void Redraw()
        {
            _scrollView.Clear();
            
            foreach (var line in _logService.GetOutputLines())
                AddLogLine(line);
        }

        public void OnAttachToPanel(AttachToPanelEvent evt)
        {
            foreach (var line in _logService.GetOutputLines())
                AddLogLine(line);
            
            _logService.OutputReceived += AddLogLine;
        }

        public void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            _scrollView.Clear();
            
            _logService.OutputReceived -= AddLogLine;
        }

        private void AddLogLine(OutputLine line)
        {
            var text = new Label(line.Text);

            if (line.IsError)
                text.AddToClassList("error");
            
            _scrollView.Add(text);
            _scrollView.ScrollTo(text);
        }
    }
}
