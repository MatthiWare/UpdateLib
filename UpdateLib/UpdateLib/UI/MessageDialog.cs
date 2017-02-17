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
    public partial class MessageDialog : Form
    {
        public string Header
        {
            get { return this.lblHeader.Text; }
            set { this.lblHeader.Text = value; }
        }

        public string Description
        {
            get { return this.lblDesc.Text; }
            set { this.lblDesc.Text = value; }
        }

        public Icon DialogIcon
        {
            set { this.pbIcon.BackgroundImage = value.ToBitmap(); }
        }

        public MessageDialog()
        {
            InitializeComponent();
        }

        public MessageDialog(String title, String header, String desc, Icon icon)
            :   this(title, header, desc, icon, MessageBoxButtons.YesNo)
        {
            
        }

        public MessageDialog(String title, String header, String desc, Icon icon, MessageBoxButtons btns)
            : this()
        {
            Header = header;
            Description = desc;
            Text = title;
            DialogIcon = icon;
        }
    }
}
