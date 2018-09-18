using System;
using System.Collections.Generic;
using System.IO;

namespace SeekAndArchive
{
    class Program
    {
        private static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        static void Main(string[] args)
        {
            String filename = args[0];
            String startPath = args[1];

            Console.WriteLine("Enter a pattern!");
            String pattern = Console.ReadLine();


            List<FileInfo> myFiles = new List<FileInfo>();

            DirectoryInfo di = new DirectoryInfo(startPath);
            if (!di.Exists)
            {

                Console.WriteLine("The specified directory does not exist.");
                return;

            }
            Search(di, filename, pattern,ref myFiles);
            foreach(FileInfo file in myFiles)
            {
                
                Console.WriteLine(file.FullName);
                FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name);
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

                watcher.EnableRaisingEvents = true;
            }
            Console.ReadKey();
           
        }


        static void Search(DirectoryInfo root, String fileName, String pattern, ref List<FileInfo> myFiles)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            files = root.GetFiles(pattern);

            if (files != null)
            {
                foreach (FileInfo fi in files)
                { 
                    if (fi.Name.Equals(fileName))
                    {
                        myFiles.Add(fi);
                    }
                }

                
                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    
                    Search(dirInfo,fileName,pattern,ref myFiles);
                }
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }
    }
}
