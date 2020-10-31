using System.IO;
using System.Linq;

namespace lab2
{
    public class Extractor
    {
        private byte[] _aesKey;
        private byte[] _aesIV;
        private string _targetDirectory;
        private Encryptor _encryptor;
        private Archivator _archivator;

        public Extractor(string targetDirectory, byte[] aesKey, byte[] aesIV)
        {
            this._targetDirectory = targetDirectory;
            this._aesKey = aesKey;
            this._aesIV = aesIV;
            this._encryptor = new Encryptor();
            this._archivator = new Archivator();
        }

        private void Encrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            var encrypted = _encryptor.EncryptStringToBytesAes(streamReader.ReadToEnd(), _aesKey, _aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(string.Concat(encrypted.Select(x => (char)x).ToArray()));
            streamWriter.Close();
            Logger.RecordEntry("encrypted", filePath);
        }
        private void Decrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            string decrypted = _encryptor.DecryptStringFromBytesAes(streamReader.ReadToEnd().Select(x => (byte)x).ToArray(), _aesKey, _aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(decrypted);
            streamWriter.Close();
            Logger.RecordEntry("decrypted", filePath);
        }

        public void Extract(string sourceFile)
        {
            ArchiveOldFiles(_targetDirectory);
            Encrypt(sourceFile);
            var sourceFileInfo = new FileInfo(sourceFile);
            string initialExtension = sourceFileInfo.Extension;
            var archivedFileInfo = new FileInfo(sourceFileInfo.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(sourceFileInfo.Name) + ".gz");
            _archivator.Compress(sourceFile, archivedFileInfo.FullName);

            sourceFileInfo.Delete();

            File.Move(archivedFileInfo.FullName, _targetDirectory + Path.DirectorySeparatorChar + archivedFileInfo.Name);
            archivedFileInfo = new FileInfo(_targetDirectory + Path.DirectorySeparatorChar + archivedFileInfo.Name);
            Logger.RecordEntry("moved", archivedFileInfo.FullName);

            sourceFileInfo = new FileInfo(archivedFileInfo.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(archivedFileInfo.Name) + initialExtension);
            _archivator.Decompress(archivedFileInfo.FullName, sourceFileInfo.FullName);
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
                    var archiveDirectory = new DirectoryInfo(Path.Combine(targetDirectory, "archive", creationDate.Year.ToString(), creationDate.Month.ToString(), creationDate.Day.ToString(), creationDate.ToString("HH.mm.ss")));
                    if (!archiveDirectory.Exists)
                    {
                        archiveDirectory.Create();
                    }
                    file.MoveTo(archiveDirectory.FullName + Path.DirectorySeparatorChar + file.Name);
                    Logger.RecordEntry("added to archieve", file.FullName);
                }
            }
        }
    }
}
