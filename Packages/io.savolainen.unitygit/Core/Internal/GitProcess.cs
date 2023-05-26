using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityGit.Core.Data;

namespace UnityGit.Core.Internal
{
    internal class GitProcess
    {
        private readonly Process _process;

        private readonly GitProcessResult _result = new GitProcessResult();

        private readonly TaskCompletionSource<bool> _processCompletionSource
            = new TaskCompletionSource<bool>();

        private int _timeoutMs = 0;

        public GitProcess(Process process)
        {
            _process = process;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            process.Exited += OnProcessExited;
            process.OutputDataReceived += OnStandardOutputReceived;
            process.ErrorDataReceived += OnStandardErrorReceived;
        }

        ~GitProcess()
        {
            Cleanup();
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _processCompletionSource.SetResult(true);
        }

        private void OnStandardOutputReceived(object sender, DataReceivedEventArgs evt)
        {
            if (evt.Data != null)
                _result.Output.Add(new OutputLine($"[git] {evt.Data}", false));
        }

        private void OnStandardErrorReceived(object sender, DataReceivedEventArgs evt)
        {
            if (evt.Data != null)
                _result.Output.Add(new OutputLine($"[git] {evt.Data}", true));
        }

        private void Cleanup()
        {
            _process.Exited -= OnProcessExited;
            _process.OutputDataReceived -= OnStandardOutputReceived;
            _process.ErrorDataReceived -= OnStandardErrorReceived;
        }

        public void SetTimeout(int timeoutMilliseconds)
        {
            _timeoutMs = timeoutMilliseconds;
        }

        public async Task<GitProcessResult> RunAsync()
        {
            using (_process)
            {
                if (!_process.Start())
                {
                    _result.Started = false;
                    _result.ExitCode = _process.ExitCode;
                    return _result;
                }

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                int delay;

                if (_timeoutMs > 0)
                    delay = _timeoutMs;
                else
                    delay = 60_000;
                
                var processTask = _processCompletionSource.Task;
                var timeoutTask = Task.Delay(delay);
                var finishTask = Task.WhenAny(processTask, timeoutTask);

                var completedTask = await finishTask;
                
                if (completedTask == processTask)
                {
                    _result.ExitCode = _process.ExitCode;
                }
                else
                {
                    _process.Kill();
                    _result.Timeout = true;
                }
            }
            
            Cleanup();

            return _result;
        }

        public override string ToString()
        {
            return $"'{_process.StartInfo.FileName} {_process.StartInfo.Arguments}'";
        }
    }
}
