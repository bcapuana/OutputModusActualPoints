using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GetFeaturesForOutput
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ProgramArguments.GetArguments(args, out ProgramArguments arguments))
            {
                ErrorExit();
                return;
            }

            if (!GetAllMeasuredFeatures(arguments, out List<string> allFeatures))
            {
                ErrorExit();
                return;
            }

            if (!WriteOutFeatures(arguments, allFeatures))
            {
                ErrorExit();
                return;
            }


        }

        private static bool WriteOutFeatures(ProgramArguments arguments, List<string> allFeatures)
        {
            using (StreamWriter sw = new StreamWriter(arguments.OutputFile))
            {
                int firstFeature = 0;
                int lastFeature = allFeatures.Count - 1;
                if (arguments.FirstFeature != string.Empty)
                {
                    firstFeature = allFeatures.FindIndex(f => f.ToUpper() == arguments.FirstFeature.ToUpper());
                    if (firstFeature == -1)
                        firstFeature = 0;
                }

                if (arguments.LastFeature != string.Empty)
                {
                    lastFeature = allFeatures.FindIndex(f => f.ToUpper() == arguments.LastFeature.ToUpper());
                    if (lastFeature == -1)
                        lastFeature = allFeatures.Count - 1;
                }

                for(int i = firstFeature; i <= lastFeature; i++)
                {
                    string feature = allFeatures[i];
                    if (!arguments.ExclusionList.Contains(feature))
                        sw.WriteLine(feature);
                }
                sw.Flush();
                sw.Close();
            }

            return true;
        }

        static void ErrorExit()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static bool GetAllMeasuredFeatures(ProgramArguments arguments, out List<string> allFeatures)
        {
            allFeatures = new List<string>();
            SqlConnection conn = null;
            if (!ConnectToDatabase(arguments, out conn)) return false;
            string query = GetSqlQuery();
            DataTable dt = new DataTable();
            using (conn)
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            foreach (DataRow r in dt.Rows)
            {
                allFeatures.Add(r[0].ToString());
            }

            return true;
        }

        private static string GetSqlQuery()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Stream resource = currentAssembly.GetManifestResourceStream("GetFeaturesForOutput.GetFeatures.sql");
            string sqlQuery = "";
            using (StreamReader sr = new StreamReader(resource))
            {
                sqlQuery = sr.ReadToEnd();
            }
            return sqlQuery;
        }

        static bool ConnectToDatabase(ProgramArguments arguments, out SqlConnection connection)
        {
            try
            {
                string connectionString = GetConnectionString(arguments);

                connection = new SqlConnection(connectionString);
                connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                connection = null;
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static string GetConnectionString(ProgramArguments arguments)
        {
            List<ModusServer> availableServers = GetAvailableServers();
            string modusServer = GetCorrectModusServer(arguments, availableServers);
            FileInfo fi = new FileInfo(arguments.ProgramName);
            string programName = $"LK_Insp_{arguments.ProgramName.Replace(fi.Extension,string.Empty)}";

            string connectionString = $"Data Source={modusServer};Initial Catalog={programName};Integrated Security=True";

            return connectionString;
        }

        private static string GetCorrectModusServer(ProgramArguments arguments, List<ModusServer> availableServers)
        {
            string serverName = availableServers[0].ServerName;
            if (arguments.ModusVersion > 0)
            {
                string versionString = arguments.ModusVersion.ToString("F2");
                int version = Convert.ToInt32(versionString.Replace(".", string.Empty));
                string temp = availableServers.Find(ms => ms.Version == version)?.ServerName;
                if (temp != null)
                    serverName = temp;
            }

            return serverName;
        }

        private static List<ModusServer> GetAvailableServers()
        {

            List<ModusServer> servers = new List<ModusServer>();

            servers.AddRange(GetAvailableServers(RegistryView.Registry64));
            servers.AddRange(GetAvailableServers(RegistryView.Registry32));

            servers.Sort((ms1, ms2) => ms2.Version.CompareTo(ms1.Version));

            return servers;
        }

        private static IEnumerable<ModusServer> GetAvailableServers(RegistryView registryView)
        {
            List<ModusServer> availableServers = new List<ModusServer>();
            string serverName = Environment.MachineName;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        if (instanceName.ToUpper().Contains("MODUS_SERVER"))
                        {
                            string name = serverName + "\\" + instanceName;
                            Console.WriteLine(name);
                            availableServers.Add(new ModusServer(name));
                        }
                    }
                }
            }
            return availableServers;
        }
    }
}
