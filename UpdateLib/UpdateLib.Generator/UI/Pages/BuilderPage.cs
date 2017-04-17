using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MatthiWare.UpdateLib.Generator.UI.Pages
{
    public partial class BuilderPage : PageControlBase
    {
        public BuilderPage()
        {
            InitializeComponent();
        }

        protected override void OnPageInitialize()
        {
            Thread.Sleep(1000);
            throw new FileNotFoundException("Config file missing");
        }
    }
}
