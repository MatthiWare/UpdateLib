/*  UpdateLib - .Net auto update library
 *  Copyright (C) 2016 - MatthiWare (Matthias Beerens)
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published
 *  by the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
            set
            {
                index = store.IndexOf(value);
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
                if (!page.IsDone || page.HasErrors)
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
