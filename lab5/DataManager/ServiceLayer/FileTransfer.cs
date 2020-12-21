using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataManager
{
    class FileTransfer : IFileTransferService
    {
        private FileInfo file;
        public async Task<string> CreateTempFileAsync(string contentOfFile)
        {
            file = new FileInfo(Path.GetTempFileName());
            var writer = new StreamWriter(file.OpenWrite());
            await writer.WriteAsync(contentOfFile);
            writer.Close();
            return file.FullName;
        }
        public async Task TranferFileToFTPAsync(string ftpFolderPath, string fileName)
        {
            try
            {
                FileInfo targetFile = new FileInfo(Path.Combine(ftpFolderPath, fileName));
                if (targetFile.Exists)
                    targetFile.Delete();
                await Task.Run(() => file.CopyTo(Path.Combine(ftpFolderPath, fileName), true));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                file.Delete();
                Thread.Sleep(100);
            }
        }
    }
}
