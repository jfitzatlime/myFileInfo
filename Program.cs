using System;
using System.IO;

namespace dirTree
{
    public class RecursiveFileSearch
    {
        static System.Collections.Specialized.StringCollection log =
            new System.Collections.Specialized.StringCollection();

        static void Main()
        {
            Console.WriteLine("Press any key to START !!! ?");
            Console.ReadKey();
            FileInfo fi = new FileInfo(@"C:\Users\jfitz\Desktop\myCSV_333pm.txt");
            FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs);

            string strToWrite = "Drive Letter , Length , Extention , Name , Creation Time , Last Write Time , Last Access Time , Directory Name ";
            Console.WriteLine(strToWrite);

            sw.WriteLine(strToWrite);

            // Start with drives if you have to search the entire computer.
            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                // Here we skip the drive if it is not ready to be read. This  C:\Users\jfitz\Desktop
                // is not necessarily the appropriate action in all scenarios.
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir, sw);
            }

            // Write out all the files that could not be processed.
            Console.WriteLine("Files with restricted access:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
            sw.Close();
        }

        static void WalkDirectoryTree(System.IO.DirectoryInfo root, StreamWriter swrit)
        {
            Console.WriteLine("WDT: " + root.FullName);
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.mp4");
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                    string strToWrite = fi.DirectoryName.Substring(0, 2) + ", "
                                      + fi.Length + ", "
                                      + fi.Extension + ", "
                                      + fi.Name + ", "
                                      + fi.CreationTime + ", "
                                      + fi.LastWriteTime + ", "
                                      + fi.LastAccessTime + ", "
                                      + fi.DirectoryName;

                    swrit.WriteLine(strToWrite);
                    Console.WriteLine(strToWrite);
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo, swrit);
                }
            }
        }
    }
}