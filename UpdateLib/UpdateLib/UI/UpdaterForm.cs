using MatthiWare.UpdateLib.Files;
using MatthiWare.UpdateLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.UI
{
    public partial class UpdaterForm : Form
    {
        private UpdateInfoFile updateInfoFile;

        public UpdaterForm()
        {
            InitializeComponent();

            lvItems.SmallImageList.Images.Add("status_done", Resources.status_done);
            lvItems.SmallImageList.Images.Add("status_download", Resources.status_download);
            lvItems.SmallImageList.Images.Add("status_error", Resources.status_error);

            
        }

        public UpdaterForm(UpdateInfoFile updateFile)
           : this()
        {
            updateInfoFile = updateFile;
            FillList();
        }

        private void FillList()
        {
            foreach (UpdateFile file in updateInfoFile.Files)
            {
                string[] data = new string[] { "", file.Name, "Ready to download", "0%"};
                ListViewItem lvItem = new ListViewItem(data, "status_download");

                lvItems.Items.Add(lvItem);
            }
        }
    }
}
