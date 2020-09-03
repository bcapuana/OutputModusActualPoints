using System;
using System.Collections.Generic;
using System.Text;

namespace GetFeaturesForOutput
{
    public class ProgramArguments
    {
        public double ModusVersion { get; set; } = 0;

        public string ProgramName { get; set; } = string.Empty;

        public List<string> ExclusionList { get; set; } = new List<string>();

        public string FirstFeature { get; set; } = string.Empty;

        public string LastFeature { get; set; } = string.Empty;

        public string OutputFile { get; set; } = @"C:\users\public\documents\MeasuredFeatures.txt";

        const string VERSION_TAG = "-VERSION",
                     PROGRAM_NAME_TAG = "-PROGRAM",
                     EXCLUSION_LIST_TAG = "-EXCLUDE",
                     FIRST_FEATURE_TAG = "-FIRST",
                     LAST_FEATURE_TAG = "-LAST";

        private ProgramArguments() { }
        public static bool GetArguments(string[] args, out ProgramArguments output)
        {
            output = new ProgramArguments();
            string currentArgument = null;
            try
            {
                foreach (string arg in args)
                {
                    currentArgument = arg;
                    string[] splitArg = arg.Split(new char[] { ':' });
                    switch (splitArg[0])
                    {
                        case VERSION_TAG:
                            output.ModusVersion = Convert.ToDouble(splitArg[1]);
                            break;
                        case PROGRAM_NAME_TAG:
                            output.ProgramName = splitArg[1];
                            break;
                        case EXCLUSION_LIST_TAG:
                            GetExclusionList(splitArg[1], ref output);
                            break;
                        case FIRST_FEATURE_TAG:
                            output.FirstFeature = splitArg[1];
                            break;
                        case LAST_FEATURE_TAG:
                            output.LastFeature = splitArg[1];
                            break;
                        default:
                            Console.WriteLine($"Unknown argument: \"{splitArg[0]}\"");
                            break;
                    }
                }

                if(output.ProgramName==string.Empty)
                {
                    Console.WriteLine("The dmis program name must be supplied wiht the -PROGRAM argument");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                if (currentArgument != null)
                    Console.WriteLine($"Error parsing argument \"{currentArgument}\"");
                output = null;
                return false;
            }

        }

        private static void GetExclusionList(string list, ref ProgramArguments output)
        {
            string[] splitString = list.Split(new char[] { ',' });
            foreach(string feature in splitString)
            {
                output.ExclusionList.Add(feature);
            }
        }
    }
}
