using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace UnityGit.Core.Services
{
    public interface ILogService
    {
        public readonly struct OutputLine
        {
            public readonly string Text;
            public readonly bool IsError;
            
            public OutputLine(string text, bool isError)
            {
                Text = text;
                IsError = isError;
            }
        }
        
        void LogMessage(string line);

        void LogError(string line);

        void LogException(Exception exception);
        
        IReadOnlyList<OutputLine> GetOutputLines();
    }
}