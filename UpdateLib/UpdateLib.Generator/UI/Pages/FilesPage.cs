using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class FilesPage : PageControlBase
    {
        public FilesPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            Thread.Sleep(3000);
        }
    }
}
