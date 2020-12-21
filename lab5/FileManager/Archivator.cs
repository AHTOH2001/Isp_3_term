using ApplicationInsights;
using System;
using System.IO;
using System.IO.Compression;

namespace FileManager
{
    public class Archivator
    {
        public bool IsNeedToLogArchivator { get; set; }
        public string CompressionLevel { get; set; }
        public void Compress(string sourceFile, string compressedFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                using (var targetStream = File.Create(compressedFile))
                {                    
                    using (var compressionStream = new GZipStream(targetStream, (CompressionLevel)Enum.Parse(typeof(CompressionLevel), CompressionLevel)))
                    {
                        sourceStream.CopyTo(compressionStream);
                        if (IsNeedToLogArchivator)
                        {
                            Logger.RecordEntryAsync(string.Format("compressed, source size: {0}  compressed size: {1}", sourceStream.Length.ToString(), targetStream.Length.ToString()), sourceFile);
                        }
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
                        if (IsNeedToLogArchivator)
                        {
                            Logger.RecordEntryAsync("decompressed", targetFile);
                        }
                    }
                }
            }
        }
    }
}
