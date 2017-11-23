namespace Renamer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "-?")
            {
                PrintHelp();
            }

            string directory = Environment.CurrentDirectory;
            string replace = args[0];
            string target = args[1];

            if (args[0].ToLower() == "--rootpath")
            {
                directory = args[1];
                replace = args[2];
                target = args[3];

            }

            RecursiveRename(directory, replace, target);
        }


        private static void PrintHelp()
        {
            Console.WriteLine(
                    "Usage: [--rootpath pathToSolution] " +
                    "stringToReplace targetString");

            Console.WriteLine(
                    " -? for help");

            Environment.Exit(0);
        }


        public static void RecursiveRename(
            string rootdir,
            string replace,
            string target)
        {
            // Handle files
            string[] files = Directory.GetFiles(rootdir);
            Console.WriteLine($"Found {files.Length} files in {rootdir}");

            foreach (string file in files)
            {
                if (Path.GetExtension(file) != "exe"
                        && Path.GetExtension(file) != "dll")
                {
                    try
                    {
                        ReplaceInFile(file, replace, target);
                        RenameFile(file, replace, target);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                                $"Trouble with {file} in {rootdir}, " +
                                $"issue: {e.Message}");

                        Environment.Exit(1);
                    }
                }
            }

            // Recurse subdirectories
            DirectoryInfo di = new DirectoryInfo(rootdir);
            List<string> dirs =
                di.EnumerateDirectories()
                .Where(d => !d.Name.StartsWith("."))
                .Select(d => d.FullName).ToList();

            foreach (string dir in dirs)
            {
                RecursiveRename(dir, replace, target);
                RenameDirectory(dir, replace, target);
            }
        }


        private static void ReplaceInFile(
                string filepath,
                string replace,
                string target)
        {
            string content = File.ReadAllText(filepath);

            if (content.Contains(replace))
            {
                string newContent = content.Replace(replace, target);
                File.WriteAllText(filepath, newContent);
            }
        }


        private static void RenameFile(
                string filepath,
                string replace,
                string target)
        {
            if (Path.GetFileName(filepath).Contains(replace))
            {
                File.Move(
                        filepath,
                        Path.Combine(
                            Path.GetDirectoryName(filepath),
                            Path.GetFileName(filepath)
                            .Replace(replace, target)));
            }
        }


        private static void RenameDirectory(
            string dir,
            string replace,
            string target)
        {
            try
            {
                string topFolder =
                    dir.Substring(
                            dir.LastIndexOf(
                                Path.DirectorySeparatorChar))
                    .Replace(
                            Path.DirectorySeparatorChar.ToString(),
                            "");

                if (topFolder.Contains(replace))
                {
                    string targetDir =
                        dir.Substring(
                                0,
                                dir.LastIndexOf(Path.DirectorySeparatorChar));

                    targetDir =
                        Path.Combine(
                                targetDir,
                                topFolder.Replace(replace, target));

                    Directory.Move(dir, targetDir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Trouble with {dir}, issue:{e.Message}");
                Environment.Exit(1);
            }
        }
    }
}
