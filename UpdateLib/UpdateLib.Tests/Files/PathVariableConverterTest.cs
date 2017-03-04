using MatthiWare.UpdateLib.Files;
using NUnit.Framework;
using System;
using System.IO;

namespace UpdateLib.Tests.Files
{
    [TestFixture]
    public class PathVariableConverterTest
    {
        private PathVariableConverter converter;

        [SetUp]
        public void Setup()
        {
            converter = new PathVariableConverter();
        }
        
        [Test]
        public void GettingAVariableReturnsTheCorrectPath()
        {
            string temp = Path.GetTempPath();
            Assert.AreEqual(temp, converter["temp"]);

            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Assert.AreEqual(appdata, converter["appdata"]);
        }

        [Test]
        public void AddingAnItemReturnsCorrectResult()
        {
            string result = "C:/projects/";
            converter.Add("projectdir", result);

            string get = converter["projectdir"];

            Assert.AreEqual(result, get);
        }
    }
}
