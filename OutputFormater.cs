using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using static Analyzer;

internal static partial class OutputFormater
{
    // Build the target output file path in a cross platform manner
    private static string DefaultOutputPath = Directory.GetCurrentDirectory() + Conversions.ToString(Path.DirectorySeparatorChar) + "Output.log";
    public static void OutputResults(SortedDictionary<string, Application> Logs)
    {
        // Start writing the output to the file
        Console.WriteLine("Populating log file to " + DefaultOutputPath + "...");
        try
        {
            WriteToFile(PopulateList(Logs));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Environment.Exit(1);
        }
        Console.WriteLine("Log file has been successfully populated.");
    }
    private static void WriteToFile(List<string> Lines)
    {
        // Appends the formated results to the output file

        File.WriteAllText(DefaultOutputPath, ""); // Clears the file contents
        using (var sw = File.AppendText(DefaultOutputPath))
        {
            foreach (var line in Lines)
                sw.WriteLine(line);
        }
    }
    private static List<string> PopulateList(SortedDictionary<string, Application> Logs)
    {
        var Lines = new List<string>();
        string Temp;
        // Get the expected column width from MaxVal dictionary
        int Column1Lenght = (int)MaxVal["application"];
        int Column2Lenght = MaxVal["origsent"].ToString().Length + 2; // the +2 is to make up for the thousands separator
        int Column3Lenght = MaxVal["termsent"].ToString().Length + 2;

        // Populate the first two lines
        Temp = "Name" + Strings.Space(Column1Lenght + 2 - 4);
        Temp += " Origsent";
        Temp += Strings.Space(Column3Lenght - Strings.Len("Termsent") + 2) + "Termsent";

        Lines.Add(Temp);

        Temp = Strings.StrDup(Column1Lenght, "-") + Strings.Space(2) + Strings.StrDup(Column2Lenght, "-") + Strings.Space(2) + Strings.StrDup(Column3Lenght, "-");
        Lines.Add(Temp);

        // Iterate throught the logs and add them to the list with the right format
        Application App;
        string origsent;
        string termsent;
        foreach (KeyValuePair<string, Application> Pair in Logs)
        {
            App = Pair.Value; // Get the value result as a structure
            origsent = ThousandSeperator(App._origsent); // get the formatted value of origsent as string
            termsent = ThousandSeperator(App._termsent); // get the formatted value of termsent as string

            Temp = Pair.Key + Strings.Space(Column1Lenght + 2 - Strings.Len(Pair.Key)); // format the first column
            Temp += Strings.Space(Column2Lenght - Strings.Len(origsent)) + origsent; // format the second column
            Temp += Strings.Space(Column3Lenght - Strings.Len(termsent) + 2) + termsent; // format the third column

            Lines.Add(Temp);
        }
        return Lines;
    }
    private static string ThousandSeperator(uint Number)
    {
        return Number.ToString("#,#");
    }
}