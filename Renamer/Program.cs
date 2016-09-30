using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Renamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "-?")
            {
                PrintHelp();
            }
            var directory = Environment.CurrentDirectory;
            var replace = args[0];
            var target = args[1];

            if (args[0].ToLower() == "--rootpath")
            {
                directory = args[1];
                replace = args[2];
                target = args[3];

            }
          
                Rename(directory, replace, target);
   
            

        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: [--rootpath pathToSolution] stringToReplace targetString");
            Console.WriteLine(" -? for help");
            Environment.Exit(0);
        }

        public static void Rename(string rootdir, string replace, string target)
        {
            var files = Directory.GetFiles(rootdir);
            Console.WriteLine(String.Format("Found {0} files in {1}", files.Length, rootdir));
            foreach (var file in files)
            {
                if (Path.GetExtension(file) != "exe" && Path.GetExtension(file) != "dll")
                {
                    try
                    {
                        var content = File.ReadAllText(file);
                        if (content.Contains(replace))
                        {
                            var newContent = content.Replace(replace, target);
                            File.WriteAllText(file, newContent);
                        }
                        if (Path.GetFileName(file).Contains(replace))
                            File.Move(file, Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(file).Replace(replace, target)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(String.Format("Trouble with {0}, in {1}, issue:{2}", file, rootdir, e.Message));
                        Environment.Exit(1);

                    }
                }
            }
            var di = new DirectoryInfo(rootdir);
            var dirs = di.EnumerateDirectories().Where(d => !d.Name.StartsWith(".")).Select(d=>d.FullName);
            foreach (var dir in dirs)
            {
                Rename(dir, replace, target);
                try
                {
                    var topFolder = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar)).Replace(Path.DirectorySeparatorChar.ToString(),"");
                    if (topFolder.Contains(replace))
                    {
                        var targetDir = dir.Substring(0, dir.LastIndexOf(Path.DirectorySeparatorChar));
                        targetDir = Path.Combine(targetDir, topFolder.Replace(replace, target));
                        Directory.Move(dir, targetDir);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Trouble with {0}, issue:{1}", dir, e.Message));
                    Environment.Exit(1);

                }
            }
        }
    }
}
