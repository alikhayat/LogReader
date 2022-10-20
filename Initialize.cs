using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static partial class Inialize
{
    private const string DefaultfileName = "syslog.txt";
    public static void Main(string[] args)
    {

        // Check if the console argument is valid if found
        // If not then check if the default file to be found
        // If the file is to be found and is not empty then append its lines to a list of strings

        var LogList = new List<string>();

        if (args.Length > 0 & args.Length <= 1)
        {
            string fileName = args[0];
            if (FilenameIsOK(fileName))
            {
                LogList = ReadFileContents(fileName);
            }
            else
            {
                Console.WriteLine("File or directory submitted is not valid. Exiting...");
                Environment.Exit(128);
            }
        }
        else if (args.Length == 0)
        {
            Console.WriteLine("No file was passed, attempting to find " + DefaultfileName);
            LogList = ReadFileContents(DefaultfileName);
        }

        // Attempt to filter results and start writing the output file

        OutputFormater.OutputResults(Analyzer.Analyze(LogList));
    }
    private static bool FilenameIsOK(string fileName)
    {
        // Checks if the provided filename and path are valid

        string file = Path.GetFileName(fileName);
        string directory = Path.GetDirectoryName(fileName);

        return !(file.Intersect(Path.GetInvalidFileNameChars()).Any() || directory.Intersect(Path.GetInvalidPathChars()).Any());
    }

    private static List<string> ReadFileContents(string fileName)
    {
        // Reads the file contents and appends them to a list of strings

        var list = new List<string>();

        if (File.Exists(fileName))
        {
            Console.WriteLine("File found attempting to read it...");
            using (var Reader = new StreamReader(fileName))
            {
                while (Reader.EndOfStream == false)
                    list.Add(Reader.ReadLine());
            }
        }
        else
        {
            Console.WriteLine(fileName + " has not been found. Exiting...");
            Environment.Exit(-1);
        }

        if (list.Count != 0)
        {
            return list;
        }
        else
        {
            Console.WriteLine("File appears to be empty. Exiting...");
            Environment.Exit(-2);
            return null;
        }
    }
}