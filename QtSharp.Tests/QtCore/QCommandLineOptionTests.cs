using NUnit.Framework;
using QtCore;

namespace QtSharp.Tests.QtCore
{
    [TestFixture]
    public class QCommandLineOptionTests
    {
        [Test]
        public void TestEmptyConstructor()
        {
            var s = new QCommandLineOption("verbose", "Verbose mode. Prints out more information.", "valName", "-option1");

            Assert.AreEqual("verbose", s.Names.Join(""));
            Assert.AreEqual("Verbose mode. Prints out more information.", s.Description);
            Assert.AreEqual("valName", s.ValueName);
            Assert.AreEqual("-option1", s.DefaultValues.Join(""));
        }
    }
}