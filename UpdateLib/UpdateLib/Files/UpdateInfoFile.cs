using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class UpdateInfoFile
    {
        public UpdateInfoFile(string version)
        {
            Files = new List<UpdateFile>();
            
            VersionString = version;
        }

        public string VersionString { get; set; }

        public List<UpdateFile> Files { get; set; }
    }
}
