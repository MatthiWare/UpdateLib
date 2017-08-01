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

            Assert.AreEqual(string.Empty, string.Empty.Split('/').AppendAll(string.Empty));
        }

        [Test]
        public void TestNotEmpty()
        {
            string test = "this/is/a/test///";

            Assert.AreEqual(7, test.Split('/').Count());
            Assert.AreEqual(4, test.Split('/').NotEmpty().Count());
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

        [Test]
        public void TestNotNullMethod()
        {
            List<TestObject> objs = new List<TestObject>();

            objs.Add(new TestObject(0));
            objs.Add(null);
            objs.Add(null);
            objs.Add(new TestObject(1));
            objs.Add(null);
            objs.Add(new TestObject(2));

            Assert.AreEqual(3, objs.NotNull().Count());
        }

        [Test]
        public void TestSkipLast0Items()
        {
            IEnumerable<int> items = Enumerable.Range(1, 20).SkipLast(0);
            Assert.AreEqual(20, items.Count());

            Assert.AreEqual(20, items.Reverse().First());


        }

        [Test]
        public void TestSkipLast1Items()
        {
            IEnumerable<int> items = Enumerable.Range(1, 20).SkipLast(1);
            Assert.AreEqual(19, items.Count());

            Assert.AreEqual(19, items.Reverse().First());
        }

        [Test]
        public void TestSkipLast2Items()
        {
            IEnumerable<int> items = Enumerable.Range(1, 20).SkipLast(2);
            Assert.AreEqual(18, items.Count());

            Assert.AreEqual(18, items.Reverse().First());

            Assert.AreEqual("1234567", Enumerable.Range(1, 10).SkipLast(3).AppendAll(string.Empty));
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
