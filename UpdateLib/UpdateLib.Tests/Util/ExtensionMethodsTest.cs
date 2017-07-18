using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatthiWare.UpdateLib.Utils;
using Moq;
using System.Diagnostics;

namespace UpdateLib.Tests.Util
{
    [TestFixture]
    public class ExtensionMethodsTest
    {
        [Test]
        public void TestAppendAll()
        {
            string test = "this is a string";

            string output = test.Split(' ').AppendAll(".");

            Assert.AreEqual("this.is.a.string", output);
        }

        [Test]
        public void TestForEach()
        {
            List<Mock<TestObject>> mocks = new List<Mock<TestObject>>();

            for (int x = 0; x <= 10; x++)
                mocks.Add(new Mock<TestObject>(x));

            mocks.Select(m => m.Object).ForEach(m => m.Start());

            foreach (var obj in mocks)
                obj.Verify(mock => mock.Start(), Times.Once);
        }

        [DebuggerDisplay("{Id}")]
        public class TestObject
        {

            public TestObject(int id)
            {
                Id = id;
            }

            public int Id { get; set; }

            public virtual void Start()
            {

            }
        }
    }
}
