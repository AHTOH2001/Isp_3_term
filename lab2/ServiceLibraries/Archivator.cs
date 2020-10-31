using System.IO;
using System.IO.Compression;

namespace lab2
{
    public class Archivator
    {
        public void Compress(string sourceFile, string compressedFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                using (var targetStream = File.Create(compressedFile))
                {
                    using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                        Logger.RecordEntry(string.Format("compressed, source size: {0}  compressed size: {1}", sourceStream.Length.ToString(), targetStream.Length.ToString()), sourceFile);
                    }
                }
            }
        }

        public void Decompress(string compressedFile, string targetFile)
        {
            using (var sourceStream = new FileStream(compressedFile, FileMode.Open))
            {
                using (var targetStream = File.Create(targetFile))
                {
                    using (var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                        Logger.RecordEntry("decompressed", targetFile);
                    }
                }
            }
        }
    }
}
