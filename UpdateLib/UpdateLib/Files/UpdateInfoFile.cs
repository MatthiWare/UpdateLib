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

        public override bool Equals(object obj)
        {
            UpdateInfoFile file = obj as UpdateInfoFile;
            if (file == null)
                return false;

            if (!file.VersionString.Equals(this.VersionString))
                return false;

            foreach (UpdateFile other in file.Files)
            {
                bool same = false;
                foreach (UpdateFile src in Files)
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

            foreach (UpdateFile file in Files)
            {
                hash += file.GetHashCode();
            }

            return hash;
        }
    }
}
