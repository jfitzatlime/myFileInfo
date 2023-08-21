using System;
// using System.IO;
// using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace dirTree
{
    public class RecursiveFileSearch
    {
        static System.Collections.Specialized.StringCollection log =
            new System.Collections.Specialized.StringCollection();

        static long msgPost = 10001;
        static string myEXT = "*.mp3";

        static string myConnStr = @"Data source=LAPTOP-DAV6VLUB\SQLEXPRESS; Initial Catalog=ctest;Persist Security Info=True;Integrated Security=true;";
        static SqlConnection conn = new SqlConnection(myConnStr);

        static long countr = 0;
        static long skipco = 0;
        static long errco = 0;
        static long fco = 0;
        static DateTime thisDate = new DateTime();

        static void Main()
        {
            Console.WriteLine("Starting");
            /*
            SimpleFileCopy sfc = new SimpleFileCopy();
            sfc.myCopyFile(@"C:\Users\jfitz\source\repos", @"C:\Users\jfitz\source\repos\testSub2", "ctestA.txt", "ctestZ.txt");
            return;
            */
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while creating table:" + e.Message + "\t" + e.GetType());
            }

            string[] drives = System.Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTreeSQL(rootDir, 0);
            }

            Console.WriteLine("Files with restricted access:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("END... Files with restricted access.");

            Console.WriteLine("Folder Countr: " + countr.ToString());
            Console.WriteLine("Error Counted: " + errco.ToString());
            Console.WriteLine("Files Skipped: " + skipco.ToString());
            Console.WriteLine("Files Counted: " + fco.ToString());
            Console.WriteLine("");
            Console.WriteLine("Press any key to finish >");
            Console.ReadKey();
            conn.Close();

        }


        static void WalkDirectoryTreeSQL(System.IO.DirectoryInfo root, int lvlDeep)
        {
            string sqlcmd;
            SqlCommand cmd;
            countr++;
            if (countr % msgPost == 0)
            {
                Console.WriteLine("Folders Scanned : " + countr.ToString() + " --- " + root.FullName);
            }

            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles(myEXT);
            }
            catch (UnauthorizedAccessException e)
            {
                errco++;
                //string sm1 = e.Message.Replace("'", "_");
                //sqlcmd = "insert into mylog (logstr, log2, logdate) values ('" + sm1 + "' , 'UnAuth' , getdate())";
                //cmd = new SqlCommand(sqlcmd, conn);
                //cmd.ExecuteNonQuery();
                // Console.WriteLine("UnAuth >>>>> " + sm1);
            }

            catch (System.IO.DirectoryNotFoundException e2)
            {
                errco++;
                string sm2 = e2.Message.Replace("'", "_");
                sqlcmd = "insert into mylog (logstr, log2, logdate) values ('" + sm2 + "' , 'DirNotFound' , getdate())";
                cmd = new SqlCommand(sqlcmd, conn);
                cmd.ExecuteNonQuery();
            }

            if (files != null)
            {
                CultureInfo cult = CultureInfo.GetCultureInfo("en-US");
                foreach (System.IO.FileInfo fi in files)
                {
                    int ii = fi.Name.IndexOf("'");
                    if (ii > 0)
                    {
                        skipco++;
                        string newf = fi.Name.Replace("'", "_");
                        string newd = fi.DirectoryName;
                        sqlcmd = "insert into mylog (logstr, log2, logdate) values ('" + newf + "' , '" + newd + "' , getdate())";
                        cmd = new SqlCommand(sqlcmd, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        ii = fi.DirectoryName.IndexOf("'");
                        if (ii > 0)
                        {
                            skipco++;
                            string newf = fi.DirectoryName.Replace("'", "_");
                            sqlcmd = "insert into mylog (logstr, log2, logdate) values ('" + newf + "' , 'Bad DirName' , getdate())";
                            cmd = new SqlCommand(sqlcmd, conn);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            fco++;
                            thisDate = DateTime.Now;
                            sqlcmd = "insert into myfile (processDate, drv, fsize, dirname, fname, fexten, credate, wridate, accdate) values (";
                            sqlcmd = sqlcmd + " '" + thisDate.ToString() + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.DirectoryName.Substring(0, 1) + "' , ";
                            sqlcmd = sqlcmd + " "  + fi.Length.ToString() + " , ";
                            sqlcmd = sqlcmd + " '" + fi.DirectoryName + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.Name + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.Extension + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.CreationTime.ToString("G", cult) + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.LastWriteTime.ToString("G", cult) + "' , ";
                            sqlcmd = sqlcmd + " '" + fi.LastAccessTime.ToString("G", cult) + "')";
                            cmd = new SqlCommand(sqlcmd, conn);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                subDirs = root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTreeSQL(dirInfo, lvlDeep + 1);
                }
            }
        }
    }
}