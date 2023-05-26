using System;
using System.Collections.Generic;
using UnityGit.Core.Data;

namespace UnityGit.Core.Services
{
    public interface ILogService
    {
        void LogMessage(string line);

        void LogError(string line);

        void LogException(Exception exception);

        void LogOutputLine(OutputLine line);
        
        IReadOnlyList<OutputLine> GetOutputLines();
    }
}
