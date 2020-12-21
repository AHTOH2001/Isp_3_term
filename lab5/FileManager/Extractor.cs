using System.IO;
using System.Linq;
using System;
using ConfigurationManager;
using ApplicationInsights;
using Models;

namespace FileManager
{
    public class Extractor
    {
        private byte[] _aesKey;
        private byte[] _aesIV;
        private string _targetDirectory;
        private Encryptor _encryptor;
        private Type _encryptorOptions;
        private Archivator _archivator;
        private Type _archivatorOptions;
        private Type _compressorOptions;

        internal Extractor(string targetDirectory, byte[] aesKey, byte[] aesIV)
        {
            this._targetDirectory = targetDirectory;
            try
            {
                _encryptorOptions = Watcher.SystemConfiguration.GetConfigurationClass(new EncryptorOptions());
                _aesKey = _encryptorOptions.GetOption<string>("aesKey").Select(c => (byte)c).ToArray();
            }
            catch
            {
                this._aesKey = aesKey;
            }
            try
            {
                _aesIV = _encryptorOptions.GetOption<string>("aesIV").Select(c => (byte)c).ToArray();
            }
            catch
            {
                this._aesIV = aesIV;
            }
            this._encryptor = new Encryptor();
            this._archivator = new Archivator();
            try
            {
                _archivatorOptions = Watcher.SystemConfiguration.GetConfigurationClass(new ArchivatorOptions());
            }
            catch
            {
                Logger.RecordStatusAsync("Warning, archivator options was not found in the config");
            }
            try
            {
                _archivator.IsNeedToLogArchivator = _archivatorOptions.GetOption<bool>("NeedToLog");
            }
            catch
            {
                Logger.RecordStatusAsync("Warning, archivator need to log option was not found in the config");
                _archivator.IsNeedToLogArchivator = true;
            }
            try
            {
                _compressorOptions = Watcher.SystemConfiguration.GetConfigurationClass(new CompressorOptions());
                _archivator.CompressionLevel = _compressorOptions.GetOption<int>("CompressingLevel").ToString();
            }
            catch
            {
                try
                {
                    _archivator.CompressionLevel = _compressorOptions.GetOption<string>("CompressingLevel");
                }
                catch
                {
                    _archivator.CompressionLevel = "Optimal";
                }
            }

            if (_archivatorOptions.GetOption<bool>("NeedToArchieve"))
            {
                ArchiveOldFiles(_targetDirectory);
            }

        }

        private void Encrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            var encrypted = _encryptor.EncryptStringToBytesAes(streamReader.ReadToEnd(), _aesKey, _aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(string.Concat(encrypted.Select(x => (char)x).ToArray()));
            streamWriter.Close();
            try
            {
                if (_encryptorOptions.GetOption<bool>("NeedToLog"))
                {
                    Logger.RecordEntryAsync("encrypted", filePath);
                }
            }
            catch
            {
                Logger.RecordEntryAsync("encrypted", filePath);
            }
        }
        private void Decrypt(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            string decrypted = _encryptor.DecryptStringFromBytesAes(streamReader.ReadToEnd().Select(x => (byte)x).ToArray(), _aesKey, _aesIV);
            streamReader.Close();
            var streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(decrypted);
            streamWriter.Close();
            try
            {
                if (_encryptorOptions.GetOption<bool>("NeedToLog"))
                {
                    Logger.RecordEntryAsync("decrypted", filePath);
                }
            }
            catch
            {
                Logger.RecordEntryAsync("decrypted", filePath);
            }
        }

        public void Extract(string sourceFile, Watcher watcher)
        {
            try
            {
                if (_archivatorOptions.GetOption<bool>("NeedToArchieve"))
                {
                    ArchiveOldFiles(_targetDirectory);
                }
            }
            catch
            {
            }
            try
            {
                if (_encryptorOptions.GetOption<bool>("NeedToEncrypt"))
                {
                    Encrypt(sourceFile);
                }
            }
            catch
            {
            }

            var sourceFileInfo = new FileInfo(sourceFile);
            string initialExtension = sourceFileInfo.Extension;
            var archivedFileInfo = new FileInfo(sourceFileInfo.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(sourceFileInfo.Name) + ".gz");
            watcher.RaisingEvents = false;
            _archivator.Compress(sourceFile, archivedFileInfo.FullName);
            sourceFileInfo.Delete();
            File.Move(archivedFileInfo.FullName, _targetDirectory + Path.DirectorySeparatorChar + archivedFileInfo.Name);
            watcher.RaisingEvents = true;

            archivedFileInfo = new FileInfo(_targetDirectory + Path.DirectorySeparatorChar + archivedFileInfo.Name);
            if (_archivator.IsNeedToLogArchivator)
            {
                Logger.RecordEntryAsync("moved", archivedFileInfo.FullName);
            }
            sourceFileInfo = new FileInfo(archivedFileInfo.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(archivedFileInfo.Name) + initialExtension);
            _archivator.Decompress(archivedFileInfo.FullName, sourceFileInfo.FullName);
            File.Delete(archivedFileInfo.FullName);
            try
            {
                if (_encryptorOptions.GetOption<bool>("NeedToEncrypt"))
                {
                    Decrypt(sourceFileInfo.FullName);
                }
            }
            catch
            {
            }
        }

        private void ArchiveOldFiles(string targetDirectory)
        {
            var directoryInfo = new DirectoryInfo(targetDirectory);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name != "archive" && file.Name.Substring(0, 3).ToLower() != "log")
                {
                    var creationDate = file.CreationTime;
                    var archiveDirectory = new DirectoryInfo(Path.Combine(targetDirectory, "archive", creationDate.Year.ToString(), creationDate.Month.ToString(), creationDate.Day.ToString(), creationDate.ToString("HH.mm")));
                    if (!archiveDirectory.Exists)
                    {
                        archiveDirectory.Create();
                    }
                    FileInfo fileInfo = new FileInfo(archiveDirectory.FullName + Path.DirectorySeparatorChar + file.Name);
                    int k = 1;
                    while (fileInfo.Exists)
                    {
                        k++;
                        string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        if (k > 2)
                            fileName = fileName.Substring(0, fileName.Length - 3);
                        fileInfo = new FileInfo(fileInfo.DirectoryName + Path.DirectorySeparatorChar + fileName + " №" + k.ToString() + fileInfo.Extension);
                    }
                    file.MoveTo(fileInfo.FullName);
                    if (_archivator.IsNeedToLogArchivator)
                    {
                        Logger.RecordEntryAsync("added to archieve", file.FullName);
                    }
                }
            }
        }
    }
}
