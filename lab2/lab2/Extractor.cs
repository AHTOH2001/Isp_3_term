using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lab2
{
    class Extractor
    {
        byte[] aesKey;
        byte[] aesIV;
        string sourceDirectory;
        string targetDirectory;
        public Extractor(string sourceDirectory, string targetDirectory, byte[] aesKey, byte[] aesIV)
        {
            this.sourceDirectory = sourceDirectory;
            this.targetDirectory = targetDirectory;
            this.aesKey = aesKey;
            this.aesIV = aesIV;
        }

        private void Encrypt(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath);
            byte[] encrypted = Encryptor.EncryptStringToBytes_Aes(streamReader.ReadToEnd(), aesKey, aesIV);
            streamReader.Close();
            StreamWriter streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(string.Concat(encrypted.Select(x => (char)x).ToArray()));
            streamWriter.Close();
            Logger.RecordEntry("зашифрован", filePath);
        }
        private void Decrypt(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string decrypted = Encryptor.DecryptStringFromBytes_Aes(streamReader.ReadToEnd().Select(x => (byte)x).ToArray(), aesKey, aesIV);
            streamReader.Close();
            StreamWriter streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(decrypted);
            streamWriter.Close();
            Logger.RecordEntry("расшифрован", filePath);
        }
        
        private string deleteExtension(string sourceFile)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);
            if (fileInfo.Extension == "") return sourceFile;
            for (int i = sourceFile.Length - 1; i >= 0; i--)
                if (sourceFile[i] == '.') return sourceFile.Substring(0, i);
            throw new Exception("deleteExtensionUnknownError");
        }
        public void Extract(string sourceFile)
        {            
            Encrypt(sourceFile);
            FileInfo sourceFileInfo = new FileInfo(sourceFile);
            string initialExtension = sourceFileInfo.Extension;
            FileInfo archivedFileInfo = new FileInfo(deleteExtension(sourceFileInfo.FullName) + ".gz");
            Archivator.Compress(sourceFile, archivedFileInfo.FullName);

            File.Move(archivedFileInfo.FullName, targetDirectory + '\\' + archivedFileInfo.Name);
            archivedFileInfo = new FileInfo(targetDirectory + '\\' + archivedFileInfo.Name);
            Logger.RecordEntry("перемещён", archivedFileInfo.FullName);

            sourceFileInfo = new FileInfo(deleteExtension(archivedFileInfo.FullName) + initialExtension);
            Archivator.Decompress(archivedFileInfo.FullName, sourceFileInfo.FullName);
            File.Delete(archivedFileInfo.FullName);
            Decrypt(sourceFileInfo.FullName);

            
        }
    }
}
