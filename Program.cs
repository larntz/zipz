using System;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace zipz
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Description = "Zip files in specified directory. \n\n*** SOURCE FILES WILL BE DELETED *** \n\nDefault options `--archive file --extension log`";

            var SourceArg = app.Argument("Path", "Path to directory containing files.").IsRequired().Accepts(v => v.ExistingDirectory());
            var FileExt = app.Option("-e|--extension <EXT>","File extension to zip. Accepts txt, csv, or log. Example: '-e txt'", CommandOptionType.SingleValue);
            FileExt.Accepts().Values("txt","csv","log");
            var ArchivePer = app.Option("-a|--archive","Create one archive per time period. Accepts file, day, month, or year.",CommandOptionType.SingleValue);
            ArchivePer.Accepts().Values("file","day","month","year");
            var DateFilter = app.Option("-d|--datefilter","Filter source files by date. Example '--datefilter 2018-03-5' for March 5, 2018. Filter will ",CommandOptionType.SingleValue);

            app.HelpOption();
            app.OnExecute(() =>
            {   
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("\n\n##### ZIPZ #####\n");
                System.Console.ResetColor();

                string sourceDir = SourceArg.Value;
                if (!sourceDir.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    sourceDir += Path.DirectorySeparatorChar; 

                System.Console.WriteLine("Source: \t{0}",sourceDir);

                string ext = "log";
                if (FileExt.HasValue())
                    ext = FileExt.Value();
                    
                System.Console.WriteLine("Extension:\t{0}",ext);

                string group = "file";
                if (ArchivePer.HasValue())
                    group = ArchivePer.Value();

                System.Console.WriteLine("Archive per: \t{0}",group);

                System.Console.WriteLine("\n\n");

                string searchPattern = "*." + ext;
                var sourceDirectory = new DirectoryInfo(sourceDir);
                var files = sourceDirectory.EnumerateFiles(searchPattern);                

                Archiver archiver = new Archiver(); 
                archiver.CreateArchives(files, group);

            });

            return app.Execute(args);
        }
    }
}
