using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using NUnit.Framework;
using MatthiWare.UpdateLib.UI;
using Moq;
using System.Windows.Forms;

namespace UpdateLib.Tests.UI
{
    [TestFixture]
    public class WizardPageCollectionTest
    {
        private WizardPageCollection wizard;

        [SetUp]
        public void Before()
        {
            wizard = new WizardPageCollection();
        }

        [Test]
        public void CorrectPageCount()
        {
            TestPage page1 = new TestPage();
            TestPage page2 = new TestPage();
            TestPage page3 = new TestPage();
            
            wizard.Add(page1);
            wizard.Add(page2);
            wizard.Add(page3);

            Assert.AreEqual(3, wizard.Count);
        }

        [Test]
        public void CorrectFirstPageAndLastPage()
        {
            TestPage page1 = new TestPage();
            TestPage page2 = new TestPage();
            TestPage page3 = new TestPage();

            wizard.Add(page1);
            wizard.Add(page2);
            wizard.Add(page3);

            Assert.AreEqual(page1, wizard.FirstPage);
            Assert.AreEqual(page3, wizard.LastPage);
        }

        [Test]
        public void AddNullPageShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { wizard.Add(null); });
        }

        [Test]
        public void ClearingTheWizardShouldReturnCountOfZero()
        {
            TestPage page1 = new TestPage();
            TestPage page2 = new TestPage();
            TestPage page3 = new TestPage();

            wizard.Add(page1);
            wizard.Add(page2);
            wizard.Add(page3);

            wizard.Clear();

            Assert.AreEqual(0, wizard.Count);
        }

        [Test]
        public void NextShouldReturnTheCorrectPage()
        {
            Mock<IWizardPage> page1 = new Mock<IWizardPage>();
            page1.SetupGet<bool>(p => p.IsBusy).Returns(false);
            page1.SetupGet<bool>(p => p.IsDone).Returns(true);
            page1.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page2 = new Mock<IWizardPage>();
            page2.SetupGet<bool>(p => p.IsBusy).Returns(false);
            page2.SetupGet<bool>(p => p.IsDone).Returns(true);
            page2.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page3 = new Mock<IWizardPage>();
            page3.SetupGet<bool>(p => p.IsBusy).Returns(false);
            page3.SetupGet<bool>(p => p.IsDone).Returns(true);
            page3.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            wizard.Add(page1.Object);
            wizard.Add(page2.Object);
            wizard.Add(page3.Object);

            Assert.AreEqual(wizard.FirstPage, wizard.CurrentPage);

            IWizardPage page = wizard.Next();

            Assert.AreEqual(page2.Object, page);

            page = wizard.Next();

            Assert.AreEqual(page3.Object, page);
            Assert.AreEqual(wizard.LastPage, page);

        }


        private class TestPage : IWizardPage
        {
            private UserControl control = new UserControl();
            public UserControl Conent
            {
                get
                {
                    return control;
                }
            }

            public bool IsBusy
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsDone
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool NeedsCancel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool NeedsExecution
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string Title
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public UpdaterForm UpdaterForm
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public event EventHandler PageUpdate;

            public void Cancel()
            {
                throw new NotImplementedException();
            }

            public void Execute()
            {
                throw new NotImplementedException();
            }

            public void PageEntered()
            {
                throw new NotImplementedException();
            }
        }

    }
}
