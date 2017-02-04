using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Files
{
    [Serializable]
    public class UpdateFile
    {
        public UpdateFile(string version)
        {
            Files = new List<FileEntry>();
            
            VersionString = version;
        }

        public string VersionString { get; set; }

        public List<FileEntry> Files { get; set; }

        public override bool Equals(object obj)
        {
            UpdateFile file = obj as UpdateFile;
            if (file == null)
                return false;

            if (!file.VersionString.Equals(this.VersionString))
                return false;

            foreach (FileEntry other in file.Files)
            {
                bool same = false;
                foreach (FileEntry src in Files)
                {
                    if (other.Equals(src))
                    {
                        same = true;
                        break;
                    }
                        
                }

                if (!same)
                    return false;
            }

            return true;

        }

        public override int GetHashCode()
        {
            int hash = 7;

            hash = (hash * 7) + VersionString.GetHashCode();

            foreach (FileEntry file in Files)
            {
                hash += file.GetHashCode();
            }

            return hash;
        }
    }
}
