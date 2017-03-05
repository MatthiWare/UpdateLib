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

        [Test]
        public void GetEmptyKeyShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { string val = converter[null]; });
            Assert.Throws<ArgumentNullException>(() => { string val = converter[""]; });
        }

        [Test]
        public void GetNonExistingKeyReturnsNull()
        {
            string key = "myrandomtestkey";
            if (converter.Contains(key))
                converter.Remove(key);

            Assert.IsNull(converter[key]);
        }

        [Test]
        public void AddDuplicateKeyThrowsArgumentException()
        {
            string key = "myrandomtestkey";
            string val = "value";

            if (converter.Contains(key))
                converter.Remove(key);

            converter.Add(key, val);

            Assert.AreEqual(val, converter[key]);

            Assert.Throws<ArgumentException>(() => { converter.Add(key, val); });
        }

        [Test]
        public void ContainsReturnsFalseForEmptyOrNullKey()
        {
            Assert.False(converter.Contains(""));
            Assert.False(converter.Contains(null));
        }

        [Test]
        public void ContainsReturnsFalseForNonExistingKey()
        {
            string key = "myrandomtestkey";

            if (converter.Contains(key))
                converter.Remove(key);

            Assert.False(converter.Contains(key));
        }

        [Test]
        public void ContainsReturnsTrueForExistingKey()
        {
            string key = "myrandomtestkey";
            if (!converter.Contains(key))
                converter.Add(key, "val");

            Assert.True(converter.Contains(key));
        }

        [Test]
        public void RemoveThrowsArgumentNullExceptionForEmptyOrNullKey()
        {
            Assert.Throws<ArgumentNullException>(() => { converter.Remove(""); });
            Assert.Throws<ArgumentNullException>(() => { converter.Remove(null); });
        }

        [Test]
        public void RemoveReturnsFalseForNonExistingKey()
        {
            string key = "mytestkey";
            if (converter.Contains(key))
                converter.Remove(key);

            Assert.False(converter.Remove(key));
        }
    }
}
