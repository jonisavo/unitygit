using NUnit.Framework;
using UnityGit.Core.Data;

namespace UnityGit.Tests.Core.Data
{
    [TestFixture]
    public class CredentialsTests
    {
        [TestCase("", "", false)]
        [TestCase("name", "", false)]
        [TestCase("", "password123", false)]
        [TestCase("name", "password123", true)]
        public void IsValid_ReturnsWhetherCredentialsAreValid(string name, string password, bool expected)
        {
            var credentials = new Credentials { Username = name, Password = password };

            Assert.That(credentials.IsValid(), Is.EqualTo(expected));
        }
    }
}
