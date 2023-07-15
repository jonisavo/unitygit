using System;
using NUnit.Framework;
using UnityGit.Core.Data;
using UnityGit.Core.Services;

namespace UnityGit.Tests.Core.Services
{
    [TestFixture]
    public class UnityGitLogServiceTests
    {
        [Test]
        public void LogMessage_AddsOutputLineToList()
        {
            var logService = new UnityGitLogService();
            var expectedLine = new OutputLine("Test message", false);
            
            logService.LogMessage("Test message");
            var outputLines = logService.GetOutputLines();
            
            Assert.AreEqual(1, outputLines.Count);
            Assert.AreEqual(expectedLine, outputLines[0]);
        }

        [Test]
        public void LogError_AddsOutputLineToList()
        {
            var logService = new UnityGitLogService();
            var expectedLine = new OutputLine("Test error", true);
            
            logService.LogError("Test error");
            var outputLines = logService.GetOutputLines();
            
            Assert.AreEqual(1, outputLines.Count);
            Assert.AreEqual(expectedLine, outputLines[0]);
        }
        
        private class TestException : Exception
        {
            public override string StackTrace { get; }

            public TestException(string message) : base(message)
            {
                StackTrace = "Stack trace";
            }
        }

        [Test]
        public void LogException_LogsExceptionDetails()
        {
            var logService = new UnityGitLogService();
            var exception = new TestException("Test exception");
            
            logService.LogException(exception);
            var outputLines = logService.GetOutputLines();
            
            Assert.AreEqual(3, outputLines.Count);
            Assert.AreEqual("TestException occurred.", outputLines[0].Text);
            Assert.AreEqual(true, outputLines[0].IsError);
            Assert.AreEqual("Test exception", outputLines[1].Text);
            Assert.AreEqual(true, outputLines[1].IsError);
            Assert.AreEqual(outputLines[2].Text, "Stack trace");
            Assert.AreEqual(true, outputLines[2].IsError);
        }
        
        [Test]
        public void LogOutputLine_EmitsOutputReceivedEvent()
        {
            var logService = new UnityGitLogService();
            var expectedLine = new OutputLine("Test message", false);
            OutputLine receivedLine = default;
            logService.OutputReceived += line => receivedLine = line;
            
            logService.LogOutputLine(expectedLine);
            
            Assert.AreEqual(expectedLine, receivedLine);
        }
    }
}
