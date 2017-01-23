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

        public override bool Equals(object obj)
        {
            UpdateFile file = obj as UpdateFile;
            if (file == null)
                return false;

            return this.Name == file.Name;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            return hash;
        }
    }
}
