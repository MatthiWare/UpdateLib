using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MatthiWare.UpdateLib.Tasks;
using MatthiWare.UpdateLib.Generator.Data;

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
            TreeViewFolderNode appFolder = new TreeViewFolderNode("Application Folder");
            TreeViewFolderNode appDataFolder = new TreeViewFolderNode("AppData");


        }
    }
}
