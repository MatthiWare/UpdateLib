using MatthiWare.UpdateLib.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace UpdateLib.Generator
{
    public partial class MainForm : Form
    {

        private DirectoryInfo applicationFolder = new DirectoryInfo("./ApplicationFolder");
        private DirectoryInfo outputFolder = new DirectoryInfo("./Output");

        

        public MainForm()
        {
            InitializeComponent();

            if (!applicationFolder.Exists)
                applicationFolder.Create();

            if (!outputFolder.Exists)
                outputFolder.Create();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Action generateAction = new Action(Generate);

            this.UseWaitCursor = true;

            generateAction.BeginInvoke(new AsyncCallback(GenerateCallback), null);
        }

        private void Generate()
        {
            UpdateGenerator generator = new UpdateGenerator();
            generator.AddDirectory(applicationFolder);
            UpdateFile file = generator.Build();

            using (Stream s = File.Open(string.Concat(outputFolder.FullName, "\\", "updatefile.xml"), FileMode.OpenOrCreate, FileAccess.Write))
            {
                XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));
                xml.Serialize(s, file);
            }
        }

        private void GenerateCallback(IAsyncResult result)
        {
            Console.WriteLine("Completed: " + result.IsCompleted);
            this.UseWaitCursor = false;

            MessageBox.Show("File generated, look in the output directory!");
        }
        
    }
}
