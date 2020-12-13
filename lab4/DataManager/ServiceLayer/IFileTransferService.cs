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
        string CreateTempFile(string contentOfFile);
        void TranferFileToFTP(string ftpFolderPath, string fileName);
    }
}
