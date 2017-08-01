using MatthiWare.UpdateLib.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib.Tests.Tasks
{
    [TestFixture]
    public class CleanUpTaskTest
    {

        private string m_folder, m_file;

        [SetUp]
        public void Before()
        {
            m_folder = $"{Path.GetTempPath()}test_{Guid.NewGuid().ToString()}";
            m_file = $"{m_folder}\\test.old.tmp";

            if (!Directory.Exists(m_folder))
                Directory.CreateDirectory(m_folder);

            if (!File.Exists(m_file))
                File.Open(m_file, FileMode.OpenOrCreate).Dispose();
        }

        [Test]
        public void TestCleanUp()
        {
            Assert.IsTrue(File.Exists(m_file));

            CleanUpTask task = new CleanUpTask(m_folder);
            task.ConfigureAwait(false);
            task.Start().AwaitTask();

            Assert.IsFalse(File.Exists(m_file));
        }

        [TearDown]
        public void CleanUp()
        {
            if (Directory.Exists(m_folder))
                Directory.Delete(m_folder);
        }

    }
}
