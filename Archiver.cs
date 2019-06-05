using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;


namespace zipz
{
    public class Archiver
    {
        public void CreateArchives(IEnumerable<FileInfo> files, string group)
        {
            Dictionary<string,List<FileInfo>> ArchiveFileList = new Dictionary<string, List<FileInfo>>();
            var archiveDate = DateTime.Now.ToString("s").Replace(":", "_");
            switch(group)
            {
                case "file":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + file.Name + ".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
                case "day":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + "day-" + 
                            file.LastWriteTime.ToString("yyyyMMdd") + "-created-" + archiveDate + ".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
                case "month":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + "month-" + 
                            file.LastWriteTime.ToString("yyyyMM") + "-created-" + archiveDate + ".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
                case "year":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + "year-" +
                            file.LastWriteTime.ToString("yyyy") + "-created-" + archiveDate + ".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
            }

            foreach (var key in ArchiveFileList.Keys)
            {
                CompressFiles(key, ArchiveFileList[key]);
            }
            System.Console.WriteLine("\n\n");
        }

        private void CompressFiles(string archivePath, List<FileInfo> files)
        {
            int filesAdded = 0;
            var archiveName = archivePath.Split("\\").Last();            
            using (FileStream zipToOpen = new FileStream(archivePath, FileMode.CreateNew)) 
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    System.Console.Write(String.Format("\r{0}: {1,7} files compressed.",archiveName,filesAdded));
                    foreach(FileInfo file in files)
                    {                        
                        archive.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.Optimal);
                        System.Console.Write(String.Format("\r{0}: {1,7} files compressed.",archiveName,++filesAdded));
                        File.Delete(file.FullName);
                    }
                }
            }
            System.Console.WriteLine();
        }
        
        private void AddFileToList(Dictionary<string,List<FileInfo>> list, string key, FileInfo fullname)
        {
            if(list.ContainsKey(key))
            {
                list[key].Add(fullname);
                return;
            }
            
            list[key] = new List<FileInfo>();
            list[key].Add(fullname);             
        }
    }

}