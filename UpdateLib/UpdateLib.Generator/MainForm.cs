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

            string filePath = string.Concat(outputFolder.FullName, "\\", "updatefile.xml");
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (Stream s = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(UpdateFile));
                    xml.Serialize(s, file);
                }
                catch (Exception e)
                {
                    throw e;
                }
                
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
