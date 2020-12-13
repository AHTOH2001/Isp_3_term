using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    class FileTransfer : IFileTransferService
    {
        private FileInfo file;
        public string CreateTempFile(string contentOfFile)
        {
            file = new FileInfo(Path.GetTempFileName());
            var writer = new StreamWriter(file.OpenWrite());
            writer.Write(contentOfFile);
            writer.Close();
            return file.FullName;
        }                
        public void TranferFileToFTP(string ftpFolderPath, string fileName)
        {
            try
            {
                file.MoveTo(Path.Combine(ftpFolderPath, fileName));
            }
            catch (Exception e)
            {
                file.Delete();
                throw e;
            }
        }
    }
}
