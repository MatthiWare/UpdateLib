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

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class InformationPage : PageControlBase
    {

        public string ApplicationName
        {
            get
            {
                return txtAppName.Text;
            }
            set
            {
                txtAppName.Text = value;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return txtAppVersion.Text;
            }
            set
            {
                txtAppVersion.Text = value;
            }
        }

        public InformationPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {

        }
    }
}
