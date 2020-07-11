using System;
using System.Collections.Generic;
using System.Text;

namespace Empedo.Utilities
{
    public static class TempusHubHelper
    {
        private static string _tempusHubBase = "https://tempushub.xyz";

        public static string PlayerUrl(int id)
            => _tempusHubBase + "/player/" + id;

        public static string MapUrl(string name)
            => _tempusHubBase + "/map/" + name;

        public static string ServerUrl(int id)
            => _tempusHubBase + "/server/" + id;
    }
}
