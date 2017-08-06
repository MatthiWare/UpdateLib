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
        public void AddingNullOrEmptyKeyAndOrValueThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { converter.Add(string.Empty, string.Empty); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add(null, string.Empty); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add(string.Empty, null); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add(null, null); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add("test", null); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add("test", string.Empty); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add(null, "test"); });
            Assert.Throws<ArgumentNullException>(() => { converter.Add(string.Empty, "test"); });
        }

        [Test]
        public void ReplacingShouldReplaceTheCorrectWord()
        {
            string key = "myrandomtestkey";
            if (converter.Contains(key))
                converter.Remove(key);

            string val = "myval";

            converter.Add(key, val);
            string input = $"C:\\mypath\\%{key}%\\test";
            string output = $"C:\\mypath\\{val}\\test";

            Assert.AreEqual(output, converter.Convert(input));
        }

        [Test]
        public void ReplacingShouldNotReplaceAnythingWhenKeyDoesNotExist()
        {
            string key = "myrandomtestkey";
            if (converter.Contains(key))
                converter.Remove(key);

            string input = $"C:\\mypath\\%{key}%\\test";
            string ouput = $"C:\\mypath\\{key}\\test";

            Assert.AreEqual(ouput, converter.Convert(input));
        }

        [Test]
        public void GetEmptyKeyShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { string val = converter[null]; });
            Assert.Throws<ArgumentNullException>(() => { string val = converter[string.Empty]; });
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
        public void AddDuplicateKeyOverrides()
        {
            string key = "myrandomtestkey";
            string val = "value";
            string val2 = "val2";

            if (converter.Contains(key))
                converter.Remove(key);

            converter[key] = val;

            Assert.AreEqual(val, converter[key]);

            converter[key] = val2;

            Assert.AreEqual(val2, converter[key]);
        }

        [Test]
        public void ContainsReturnsFalseForEmptyOrNullKey()
        {
            Assert.False(converter.Contains(string.Empty));
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
            Assert.Throws<ArgumentNullException>(() => { converter.Remove(string.Empty); });
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
