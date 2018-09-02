using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.UpdateLib.Utils;
using NUnit.Framework;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void TestNotNullThrowsExceptionWhenNull()
        {
            Assert.That(() => Guard.NotNull<object>(null, "null"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestNotNullOrEmptyThrowsExceptionWhenNullOrEmpty()
        {
            Assert.That(() => Guard.NotNullOrEmpty(null, "null"), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => Guard.NotNullOrEmpty("", "null"), Throws.TypeOf<ArgumentNullException>());
        }

    }
}
