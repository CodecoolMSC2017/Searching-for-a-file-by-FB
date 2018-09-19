using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SeekAndArchive
{
    class Program
    {
        private static List<FileInfo> myFiles = new List<FileInfo>();
        private static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private static DirectoryInfo archiveDir;
        static void Main(string[] args)
        {
            String filename = args[0];
            String startPath = args[1];

            Console.WriteLine("Enter a pattern!");
            String pattern = Console.ReadLine();


            

            DirectoryInfo di = new DirectoryInfo(startPath);
            if (!di.Exists)
            {

                Console.WriteLine("The specified directory does not exist.");
                return;

            }
            Search(di, filename, pattern,ref myFiles);
            foreach (FileInfo file in myFiles)
            {

                Console.WriteLine(file.FullName);
                CreateFileWatcher(file);
            }
            archiveDir = Directory.CreateDirectory("archivedFiles");
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
            FileSystemWatcher senderWatcher = (FileSystemWatcher)source;
            int index = watchers.IndexOf(senderWatcher, 0);
            ArchiveFile(myFiles[index], index);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            FileSystemWatcher senderWatcher = (FileSystemWatcher)source;
            int index = watchers.IndexOf(senderWatcher, 0);
            ArchiveFile(myFiles[index], index);
        }

        private  static void CreateFileWatcher(FileInfo file)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.EnableRaisingEvents = true;
            watchers.Add(watcher);
        }

        private static void ArchiveFile( FileInfo fileToArchive, int index)
        {
            FileStream input = fileToArchive.OpenRead();
            FileStream output = File.Create(archiveDir.FullName + @"\" + fileToArchive.Name + index + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();
            while (b != -1)
            {

                Compressor.WriteByte((byte)b);

                b = input.ReadByte();

            }
            Compressor.Close();
            input.Close();
            output.Close();
        }

        }
    }
