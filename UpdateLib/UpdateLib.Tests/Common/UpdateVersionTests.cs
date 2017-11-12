using MatthiWare.UpdateLib.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Common
{
    [TestFixture]
    public class UpdateVersionTests
    {

        [Test]
        public void TestTryParseGood()
        {
            string input = "1.2.3-beta";

            var v = new UpdateVersion(input);

            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Patch);
            Assert.AreEqual(VersionLabel.Beta, v.Label);
        }

        [Test]
        public void TestTryParseReturnsFalseInBadCase()
        {
            string input = "1.2.3.beta";

            UpdateVersion v;
            Assert.IsFalse(UpdateVersion.TryParse(input, out v));
        }

        [Test]
        public void TestStringValue()
        {
            var v = new UpdateVersion(1, 2, 3, VersionLabel.RC);

            Assert.AreEqual("1.2.3-rc", v.Value);

            v.Value = "3.1.2";

            Assert.AreEqual(3, v.Major);
            Assert.AreEqual(1, v.Minor);
            Assert.AreEqual(2, v.Patch);
            Assert.AreEqual(VersionLabel.None, v.Label);
        }

        [Test]
        public void ConstructorThrowsException()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() => new UpdateVersion(-1));
            Assert.Catch<ArgumentOutOfRangeException>(() => new UpdateVersion(1, -1));
            Assert.Catch<ArgumentOutOfRangeException>(() => new UpdateVersion(1, 1, -1));
            Assert.Catch<ArgumentException>(() => new UpdateVersion("blabla"));
        }

        [Test]
        public void TestOperators()
        {
            UpdateVersion v1 = new UpdateVersion(1);
            UpdateVersion v2 = new UpdateVersion(1);
            UpdateVersion v3 = new UpdateVersion(1, 1);
            UpdateVersion v4 = new UpdateVersion(1, 1, 1);
            UpdateVersion v5 = new UpdateVersion(1, 1, 1, VersionLabel.Alpha);
            UpdateVersion v6 = new UpdateVersion(1, 1, 1, VersionLabel.Beta);
            UpdateVersion v7 = new UpdateVersion(1, 1, 1, VersionLabel.RC);

            Assert.IsTrue(v1 == v2, "v1 == v2");
            Assert.IsTrue(v1 != v3, "v1 != v3");

            Assert.IsTrue(v3 > v1, "v3 > v1");
            Assert.IsFalse(v4 < v3, "v4 < v3");

            Assert.IsTrue(v7 > v6, "v7 > v6");
            Assert.IsTrue(v6 > v5, "v6 > v5");
        }
    }
}
