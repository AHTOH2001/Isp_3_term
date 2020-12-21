using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    interface IFileTransferService
    {
        Task<string> CreateTempFileAsync(string contentOfFile);
        Task TranferFileToFTPAsync(string ftpFolderPath, string fileName);
    }
}
