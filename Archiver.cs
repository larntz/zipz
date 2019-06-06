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
                            file.LastWriteTime.ToString("yyyyMMdd") +".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
                case "month":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + "month-" + 
                            file.LastWriteTime.ToString("yyyyMM") +".zip";
                        AddFileToList(ArchiveFileList, archiveName, file);
                    }
                    break;
                case "year":
                    foreach (var file in files)
                    {
                        string archiveName = file.DirectoryName + Path.DirectorySeparatorChar.ToString() + "year-" +
                            file.LastWriteTime.ToString("yyyy") +".zip";
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
            bool existingArchive = false;
            var archiveName = archivePath.Split("\\").Last();      
            if (File.Exists(archivePath))
            {
                File.Move(archivePath, archivePath + ".existing");
                existingArchive = true;
            }      
            using (FileStream zipToOpen = new FileStream(archivePath, FileMode.CreateNew)) 
            using (ZipArchive newArchive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                if(existingArchive)
                {
                    // zip file inception... copying entries from the .existing zip file into a new 
                    //  archive. *** Doing this so we aren't loading the entire existing archive into RAM (could be yuuuuge!)
                    // We: 1. open a stream to the existing archive (copyFromArchive), 2. open the zip archive,
                    //  3. foreach entry in the copyFromArchive we create a new entry in the new archive,
                    //  4. and read the entry data from existing entry into the new entry.
                    System.Console.WriteLine("Existing archive found. Copying to new archive...");
                    using (FileStream existingZipFile = new FileStream(archivePath + ".existing", FileMode.Open, FileAccess.Read))
                    using (ZipArchive copyFromArchive = new ZipArchive(existingZipFile, ZipArchiveMode.Read))
                        foreach (var oldEntry in copyFromArchive.Entries)
                        {                                    
                            var newEntry = newArchive.CreateEntry(oldEntry.FullName);
                            using (Stream toStream = newEntry.Open())
                            using (Stream fromStream = oldEntry.Open())
                                fromStream.CopyTo(toStream);
                        }           
                    File.Delete(archivePath + ".existing");
                }

                System.Console.Write(String.Format("\r{0}: {1,7} files compressed.",archiveName,filesAdded));
                foreach(FileInfo file in files)
                {                        
                    newArchive.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.Optimal);
                    System.Console.Write(String.Format("\r{0}: {1,7} files compressed.",archiveName,++filesAdded));
                    File.Delete(file.FullName);
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