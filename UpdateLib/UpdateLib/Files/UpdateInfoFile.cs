using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class UpdateInfoFile
    {
        public UpdateInfoFile(String version)
        {
            VersionString = version;
        }

        public String VersionString { get; set; }
    }
}
