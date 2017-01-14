using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class UpdateFile
    {

        public string Name { get; set; }

        public UpdateFile()
        { }

        public UpdateFile(string name)
        {
            Name = name;
        }

    }
}
