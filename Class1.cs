using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dirTree
{
    // Simple synchronous file copy operations with no user interface.
    // To run this sample, first create the following directories and files:
    // C:\Users\Public\TestFolder
    // C:\Users\Public\TestFolder\test.txt
    // C:\Users\Public\TestFolder\SubDir\test.txt
    public class SimpleFileCopy
    {
        public void myCopyFile(string fromFolder, string toFolder, string fromFile, string toFile)
        {
            string sourceFile = System.IO.Path.Combine(fromFolder, fromFile);
            string destFile = System.IO.Path.Combine(toFolder, toFile);

            if (!System.IO.Directory.Exists(toFolder))
                System.IO.Directory.CreateDirectory(toFolder);

            System.IO.File.Copy(sourceFile, destFile, true);
        }


    }
}
