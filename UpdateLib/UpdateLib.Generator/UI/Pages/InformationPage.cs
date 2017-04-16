using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    }
}
