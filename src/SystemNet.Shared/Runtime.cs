using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Shared
{

    
    public static class Runtime
    {
        public  static string SecreteKey = "28008307-bd68-4954-agd0-a992c481a521";
        public static string[,] connectionStrings;
        public static string DataPrimeiraReuniao { get; set; }
        public static string NameSystem { get; set; }
        public static string Smtp { get; set; }
        public static string Port { get; set; }
        public static string User { get; set; }
        public static string Sender { get; set; }
        public static string Password { get; set; }
        public static string Ssl { get; set; }
        public static string TimeOutClientSmtp { get; set; }


        public static string GetConnectionString(string instance)
        {
            for (int i = 0; i < connectionStrings.GetLength(0); i++)
            {
                if (connectionStrings[i, 0] == instance)
                {
                    return connectionStrings[i, 1];
                }
            }
            throw new Exception("Runtime Shared: Connection string not found");
        }


    }
}
