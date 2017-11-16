using MatthiWare.UpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Compression.GZip
{
    public static class GZip
    {

        public static void Decompress(Stream inStream, Stream outStream)
        {
            if (inStream == null || outStream == null)
                throw new ArgumentNullException("Streams");

            using (var gzip = new GZipInputStream(inStream))
                IOUtils.Copy(gzip, outStream, new byte[4096]);
        }

        public static void Decompress(Stream inStream, Stream outStream, int level)
        {
            if (inStream == null || outStream == null)
                throw new ArgumentNullException("Streams");

            using (var gzip = new GZipOutputStream(outStream, level))
                IOUtils.Copy(inStream, gzip, new byte[4096]);
        }
    }
}
