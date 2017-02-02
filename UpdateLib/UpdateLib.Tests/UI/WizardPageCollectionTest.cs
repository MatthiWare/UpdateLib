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
            

            Assert.AreEqual(3, wizard.Count);
        }

        [Test]
        public void CorrectFirstPageAndLastPage()
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

            Assert.AreEqual(page1.Object, wizard.FirstPage);
            Assert.AreEqual(page3.Object, wizard.LastPage);
        }

        [Test]
        public void AddNullPageShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { wizard.Add(null); });
        }

        [Test]
        public void ClearingTheWizardShouldReturnCountOfZero()
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

            page = wizard.Next();

            Assert.AreEqual(null, page);

        }

        [Test]
        public void PreviousShouldReturnTheCorrectPage()
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

            IWizardPage page = wizard.Next();
            page = wizard.Next();

            Assert.AreEqual(page3.Object, page);

            page = wizard.Previous();
            Assert.AreEqual(page2.Object, page);

            page = wizard.Previous();
            Assert.AreEqual(page1.Object, page);

            page = wizard.Previous();
            Assert.AreEqual(null, page);
        }

        [Test]
        public void AllDoneShouldReturnTrueWhenAllPagesAreDone()
        {
            Mock<IWizardPage> page1 = new Mock<IWizardPage>();
            page1.SetupGet<bool>(p => p.IsDone).Returns(true);
            page1.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page2 = new Mock<IWizardPage>();
            page2.SetupGet<bool>(p => p.IsDone).Returns(true);
            page2.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page3 = new Mock<IWizardPage>();
            page3.SetupGet<bool>(p => p.IsDone).Returns(true);
            page3.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            wizard.Add(page1.Object);
            wizard.Add(page2.Object);
            wizard.Add(page3.Object);

            Assert.IsTrue(wizard.AllDone());
        }

        [Test]
        public void AllDoneShouldReturnFalseWhenSomePagesAreNotDone()
        {
            Mock<IWizardPage> page1 = new Mock<IWizardPage>();
            page1.SetupGet<bool>(p => p.IsDone).Returns(true);
            page1.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page2 = new Mock<IWizardPage>();
            page2.SetupGet<bool>(p => p.IsDone).Returns(false);
            page2.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page3 = new Mock<IWizardPage>();
            page3.SetupGet<bool>(p => p.IsDone).Returns(true);
            page3.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            wizard.Add(page1.Object);
            wizard.Add(page2.Object);
            wizard.Add(page3.Object);

            Assert.IsFalse(wizard.AllDone());
        }


        [Test]
        public void CheckIfWizardContainsPage()
        {
            Mock<IWizardPage> page1 = new Mock<IWizardPage>();
            page1.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page2 = new Mock<IWizardPage>();
            page2.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> page3 = new Mock<IWizardPage>();
            page3.SetupGet<UserControl>(p => p.Conent).Returns(new UserControl());

            Mock<IWizardPage> pageNotAdded = new Mock<IWizardPage>();

            wizard.Add(page1.Object);
            wizard.Add(page2.Object);
            wizard.Add(page3.Object);

            Assert.IsTrue(wizard.Contains(page1.Object));
            Assert.IsTrue(wizard.Contains(page2.Object));
            Assert.IsTrue(wizard.Contains(page3.Object));
            Assert.IsFalse(wizard.Contains(pageNotAdded.Object));
        }

    }
}
