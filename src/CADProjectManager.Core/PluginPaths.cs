using System;
using System.IO;

namespace CADProjectManager.Core
{
    public static class PluginPaths
    {
        public static string LocalRoot
        {
            get
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // Path.Combine("", "CADProjectManager") is relative and could land in
                // AutoCAD's current working directory. Never allow that fallback.
                if (string.IsNullOrEmpty(localAppData) || !Path.IsPathRooted(localAppData))
                {
                    localAppData = Path.GetTempPath();
                }

                return Path.Combine(localAppData, "CADProjectManager");
            }
        }

        public static string ConfigDirectory
        {
            get { return Path.Combine(LocalRoot, "config"); }
        }

        public static string LogsDirectory
        {
            get { return Path.Combine(LocalRoot, "logs"); }
        }

        public static string LogFileFor(DateTime localDate)
        {
            return Path.Combine(LogsDirectory, localDate.ToString("yyyy-MM-dd") + ".log");
        }
    }
}
