using System;
using System.Collections.Generic;
using UnityGit.Core.Data;

namespace UnityGit.Core.Services
{
    public sealed class UnityGitLogService : ILogService
    {
        private readonly List<OutputLine> _outputLines =
            new List<OutputLine>(30);

        public delegate void OutputReceivedDelegate(OutputLine line);
        public event OutputReceivedDelegate OutputReceived;

        public void LogMessage(string line)
        {
            var outputLine = new OutputLine(line, false);
            LogOutputLine(outputLine);
        }

        public void LogError(string line)
        {
            var outputLine = new OutputLine(line, true);
            LogOutputLine(outputLine);
        }

        public void LogException(Exception exception)
        {
            LogError($"{exception.GetType().Name} occurred.");
            if (!string.IsNullOrEmpty(exception.Message))
                LogError(exception.Message);
            LogError(exception.StackTrace);
        }

        public void LogOutputLine(OutputLine outputLine)
        {
            _outputLines.Add(outputLine);
            OutputReceived?.Invoke(outputLine);
        }

        public IReadOnlyList<OutputLine> GetOutputLines()
        {
            return _outputLines;
        }
    }
}
