using System;
using System.Collections.Generic;
using LibGit2Sharp;
using UnityEngine;

namespace UnityGit.Core.Services
{
    public class UnityGitLogService : ILogService
    {
        private readonly List<ILogService.OutputLine> _outputLines =
            new List<ILogService.OutputLine>(30);

        public delegate void OutputReceivedDelegate(ILogService.OutputLine line);
        public event OutputReceivedDelegate OutputReceived;

        public void LogMessage(string line)
        {
            Debug.Log(line);
            var outputLine = new ILogService.OutputLine(line, false);
            _outputLines.Add(outputLine);
            OutputReceived?.Invoke(outputLine);
        }

        public void LogError(string line)
        {
            Debug.LogError(line);
            var outputLine = new ILogService.OutputLine(line, true);
            _outputLines.Add(outputLine);
            OutputReceived?.Invoke(outputLine);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
            LogError($"{exception.GetType().Name} occurred.");
            if (!string.IsNullOrEmpty(exception.Message))
                LogError(exception.Message);
            LogError(exception.StackTrace);
        }

        public IReadOnlyList<ILogService.OutputLine> GetOutputLines()
        {
            return _outputLines;
        }
    }
}