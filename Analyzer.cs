using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal static partial class Analyzer
{
    // instantiate our Regex queries for them to be compiled as it would affect performance if not. 
    private static Regex RegEx1 = new Regex(@"(?<=application=)([^\s]+)");
    private static Regex RegEx2 = new Regex(@"(?<=origsent=)([^\s]+)");
    private static Regex RegEx3 = new Regex(@"(?<=termsent=)([^\s]+)");

    public static Application App;

    private static SortedDictionary<string, Application> Logs = new SortedDictionary<string, Application>();
    public static Dictionary<string, uint> MaxVal = new Dictionary<string, uint>();
    public partial struct Application
    {
        internal uint _origsent;
        internal uint _termsent;
    }

    public static SortedDictionary<string, Application> Analyze(List<string> RawData)
    {
        // Start the log filtering process
        Console.WriteLine("Filtering results...");
        MaxValIntiate();
        FilterResults(RawData);
        return Logs;
    }
    private static void FilterResults(List<string> Rawdata)
    {
        // Loop throught the list of lines to filter throught the desired values

        string Appname;

        for (int i = 0, loopTo = Rawdata.Count - 1; i <= loopTo; i++)
        {
            if (Rawdata[i].Contains("event=application_end"))
            {
                Appname = FetchValues(Rawdata[i], 1);
                App._origsent = (uint)Convert.ToInt32(FetchValues(Rawdata[i], 2));
                App._termsent = (uint)Convert.ToInt32(FetchValues(Rawdata[i], 3));

                // Check if the key already exists in the dictionary
                // If it exists we sum the values
                // if not we append the values to the dictionary
                if (Logs.ContainsKey(Appname))
                {
                    SumValues(Appname, App._origsent, App._termsent);
                }
                else
                {
                    Logs.Add(Appname, App);
                }
                AssignLength(Appname, Logs[Appname]._origsent, Logs[Appname]._termsent);
            }
        }
    }
    private static string FetchValues(string Data, short Query)
    {
        // Uses Regular expressions to extract "application", "origsent" and "termsent" values.

        var Match = default(Match);

        switch (Query)
        {
            case 1:
                {
                    Match = RegEx1.Match(Data);
                    break;
                }
            case 2:
                {
                    Match = RegEx2.Match(Data);
                    break;
                }
            case 3:
                {
                    Match = RegEx3.Match(Data);
                    break;
                }
        }

        if (Match.Success)
        {
            return Match.Value;
        }
        else
        {
            return string.Empty;
        }

    }
    private static void AssignLength(string application, uint origsent, uint termsent)
    {
        // Determines how much wide each column should be

        try
        {
            if (application.Length > MaxVal["application"])
            {
                MaxVal["application"] = (uint)application.Length;
            }
            if (origsent > MaxVal["origsent"])
            {
                MaxVal["origsent"] = origsent;
            }
            if (termsent > MaxVal["termsent"])
            {
                MaxVal["termsent"] = termsent;
            }
        }
        catch (Exception ex)
        {

        }
    }
    private static void SumValues(string application, uint origsent, uint termsent)
    {
        // Looks up the key in the dictionary and sums up the new given values with the old ones
        Application _app;

        _app = Logs[application];
        _app._origsent += origsent;
        _app._termsent += termsent;
        Logs[application] = _app;
    }
    private static void MaxValIntiate()
    {
        MaxVal.Add("application", 0U);
        MaxVal.Add("origsent", 0U);
        MaxVal.Add("termsent", 0U);
    }
}