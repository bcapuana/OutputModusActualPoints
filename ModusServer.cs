using System;

namespace GetFeaturesForOutput
{
    public class ModusServer
    {
        public string ServerName { get; set; }
        public int Version { get; set; }
        
        public ModusServer(string serverName)
        {
            ServerName = serverName;
            string versionString = serverName.Substring(serverName.Length - 4);
            Version = Convert.ToInt32(versionString);
        }


    }
}