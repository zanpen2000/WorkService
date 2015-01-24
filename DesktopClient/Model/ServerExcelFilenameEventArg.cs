using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient.Model
{
    public class ServerExcelFilenameEventArg:EventArgs
    {
        public string ExcelFilename { get; private set; }
        public ServerExcelFilenameEventArg(string filename)
        {
            this.ExcelFilename = filename;
        }
    }
}
