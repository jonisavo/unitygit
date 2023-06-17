using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityGit.Core.Internal;
using Debug = UnityEngine.Debug;

namespace UnityGit.Tests.Core
{
    [TestFixture]
    public class GitProcessTests
    {
        [Test]
        public void It_Should_Return_Proper_String_Representation()
        {
            using var process = new Process();
            
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = "--version";
            
            var gitProcess = new GitProcess(process);
            
            Assert.That(gitProcess.ToString(), Is.EqualTo("'git --version'"));
        }

        [UnityTest]
        public IEnumerator It_Runs_The_Git_Command()
        {
            using var process = new Process();
            
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            var gitProcess = new GitProcess(process);
            
            var task = gitProcess.Run();
            
            while (!task.IsCompleted)
                yield return null;

            var result = task.Result;

            Assert.That(result.ExitCode, Is.EqualTo(0));
            Assert.That(result.Started, Is.True);
            Assert.That(result.Output.Count, Is.GreaterThan(0));
            Assert.That(result.Output[0].Text, Contains.Substring("git version"));
            Assert.That(result.Output[0].IsError, Is.False);
            Assert.That(result.Timeout, Is.False);
        }
        
        [UnityTest]
        public IEnumerator It_Times_Out_After_Given_Milliseconds()
        {
            using var process = new Process();

            string fileName;
            string arguments;
            int expectedLines;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows's cmd.exe has the timeout command, but
                // it does not work with input redirection.
                // Ping is used instead, which is pretty hacky.
                fileName = "ping";
                arguments = "-n 5 127.0.0.1";
                expectedLines = 3;
            }
            else
            {
                fileName = "sleep";
                arguments = "5";
                expectedLines = 0;
            }

            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            var gitProcess = new GitProcess(process);
            gitProcess.SetTimeout(1);
            
            var task = gitProcess.Run();
            
            while (!task.IsCompleted)
                yield return null;

            var result = task.Result;

            Assert.That(result.ExitCode, Is.Null);
            Assert.That(result.Started, Is.True);
            Assert.That(result.Output.Count, Is.EqualTo(expectedLines));
            Assert.That(result.Timeout, Is.True);
        }

        [UnityTest]
        public IEnumerator It_Outputs_Error_Lines()
        {
            using var process = new Process();
            
            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = "foobar";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            var gitProcess = new GitProcess(process);
            
            var task = gitProcess.Run();
            
            while (!task.IsCompleted)
                yield return null;

            var result = task.Result;

            Assert.That(result.ExitCode, Is.EqualTo(1));
            Assert.That(result.Started, Is.True);
            Assert.That(result.Output.Count, Is.GreaterThan(0));
            Assert.That(result.Output[0].IsError, Is.True);
            Assert.That(result.Timeout, Is.False);
        }
    }
}
