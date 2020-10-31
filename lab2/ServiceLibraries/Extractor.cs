using System;
using System.IO;
using System.Linq;

namespace lab2
{
    public class Extractor
    {
        byte[] aesKey;
        byte[] aesIV;
        string targetDirectory;
        Encryptor encryptor;
        Archivator archivator;
        public Extractor(string targetDirectory, byte[] aesKey, byte[] aesIV)
        {
            this.targetDirectory = targetDirectory;
            this.aesKey = aesKey;
            this.aesIV = aesIV;
            this.encryptor = new Encryptor();
            this.archivator = new Archivator();
        }

        private void Encrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            byte[] encrypted = encryptor.EncryptStringToBytes_Aes(streamReader.ReadToEnd(), aesKey, aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(string.Concat(encrypted.Select(x => (char)x).ToArray()));
            streamWriter.Close();
            Logger.RecordEntry("encrypted", filePath);
        }
        private void Decrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            string decrypted = encryptor.DecryptStringFromBytes_Aes(streamReader.ReadToEnd().Select(x => (byte)x).ToArray(), aesKey, aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(decrypted);
            streamWriter.Close();
            Logger.RecordEntry("decrypted", filePath);
        }

        private string DeleteExtension(string sourceFile)
        {
            var fileInfo = new FileInfo(sourceFile);
            if (fileInfo.Extension == "") return sourceFile;
            for (int i = sourceFile.Length - 1; i >= 0; i--)
                if (sourceFile[i] == '.') return sourceFile.Substring(0, i);
            throw new Exception("deleteExtensionUnknownError");
        }
        public void Extract(string sourceFile)
        {
            ArchiveOldFiles(targetDirectory);
            Encrypt(sourceFile);
            var sourceFileInfo = new FileInfo(sourceFile);
            string initialExtension = sourceFileInfo.Extension;
            var archivedFileInfo = new FileInfo(DeleteExtension(sourceFileInfo.FullName) + ".gz");
            archivator.Compress(sourceFile, archivedFileInfo.FullName);

            sourceFileInfo.Delete();

            File.Move(archivedFileInfo.FullName, targetDirectory + '\\' + archivedFileInfo.Name);
            archivedFileInfo = new FileInfo(targetDirectory + '\\' + archivedFileInfo.Name);
            Logger.RecordEntry("moved", archivedFileInfo.FullName);

            sourceFileInfo = new FileInfo(DeleteExtension(archivedFileInfo.FullName) + initialExtension);
            archivator.Decompress(archivedFileInfo.FullName, sourceFileInfo.FullName);
            File.Delete(archivedFileInfo.FullName);
            Decrypt(sourceFileInfo.FullName);
        }

        private void ArchiveOldFiles(string targetDirectory)
        {
            var directoryInfo = new DirectoryInfo(targetDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name != "archive" && file.Name.Substring(0, 3).ToLower() != "log")
                {
                    var creationDate = file.CreationTime;
                    var archiveDirectory = new DirectoryInfo(targetDirectory + "\\archive\\" + creationDate.Year.ToString() + '\\' + creationDate.Month.ToString() + '\\' + creationDate.Day.ToString() + '\\' + creationDate.ToString("HH.mm.ss"));
                    archiveDirectory.Create();
                    file.MoveTo(archiveDirectory.FullName + '\\' + file.Name);
                    Logger.RecordEntry("added to archieve", file.FullName);
                }
            }
        }
    }
}
