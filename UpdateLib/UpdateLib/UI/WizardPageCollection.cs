using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.UpdateLib.UI
{
    public class WizardPageCollection : IList<IWizardPage>
    {
        private List<IWizardPage> store;

        private int index = 0;

        public IWizardPage CurrentPage
        {
            get
            {
                return this[index];
            }
        }

        public IWizardPage FirstPage { get { return this.First(); } }

        public IWizardPage LastPage { get { return this.Last(); } }

        

        public WizardPageCollection() { store = new List<IWizardPage>(5); }

        public void Cancel()
        {

        }

        public IWizardPage Next()
        {
            if (CurrentPage == LastPage)
                return null;

            if (CurrentPage.IsBusy || !CurrentPage.IsDone)
                return null;
            
            index++;
            return CurrentPage;
        }

        public IWizardPage Previous()
        {
            if (CurrentPage == FirstPage)
                return null;

            //if (CurrentPage.IsBusy || !CurrentPage.IsDone)
            //    return null;

            index--;
            return CurrentPage;
        }

        public bool AllDone()
        {
            foreach (IWizardPage page in store)
            {
                if (!page.IsDone)
                    return false;
            }
            return true;
        }

        #region IList<IWizardPage> Implementation

        public int Count
        {
            get
            {
                return store.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IWizardPage this[int index]
        {
            get
            {
                return store[index];
            }

            set
            {
                store[index] = value;
            }
        }

        public int IndexOf(IWizardPage item)
        {
            return store.IndexOf(item);
        }

        public void Insert(int index, IWizardPage item)
        {
            store.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            store.RemoveAt(index);
        }

        public void Add(IWizardPage item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.Conent.Dock = System.Windows.Forms.DockStyle.Fill;
            store.Add(item);
        }

        public void Clear()
        {
            index = 0;
            store.Clear();
        }

        public bool Contains(IWizardPage item)
        {
            return store.Contains(item);
        }

        public void CopyTo(IWizardPage[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        public bool Remove(IWizardPage item)
        {
           return store.Remove(item);
        }

        public IEnumerator<IWizardPage> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return store.GetEnumerator();
        }

        #endregion
    }
}
