using System;
using System.IO;
using System.Text;
using CADProjectManager.Core;

namespace CADProjectManager.Infrastructure
{
    public static class BootstrapLogger
    {
        private static readonly object Sync = new object();
        private static readonly Encoding LogEncoding = new UTF8Encoding(false);
        private static bool _initialized;
        private static string _logPath;

        public static bool IsAvailable
        {
            get { return _initialized && !string.IsNullOrEmpty(_logPath); }
        }

        public static string LogPath
        {
            get { return _logPath ?? PluginPaths.LogFileFor(DateTime.Now); }
        }

        public static void Initialize()
        {
            try
            {
                lock (Sync)
                {
                    if (_initialized)
                    {
                        return;
                    }

                    Directory.CreateDirectory(PluginPaths.LogsDirectory);
                    _logPath = PluginPaths.LogFileFor(DateTime.Now);
                    _initialized = true;
                    WriteLineUnsafe("INFO", "CADProjectManager loaded.");
                }
            }
            catch
            {
                // Logging must never prevent AutoCAD from loading the plugin.
                _initialized = false;
                _logPath = null;
            }
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Error(string message, Exception exception)
        {
            if (exception == null)
            {
                Write("ERROR", message);
                return;
            }

            Write("ERROR", Collapse((message ?? string.Empty) + " | " + exception));
        }

        public static void Flush()
        {
            // Each record is appended and closed synchronously. There is no background writer to flush.
        }

        private static string Collapse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\n");
        }

        private static void Write(string level, string message)
        {
            try
            {
                lock (Sync)
                {
                    if (!_initialized)
                    {
                        return;
                    }

                    // AutoCAD sessions may remain open across midnight; rotate to the current local date.
                    var current = PluginPaths.LogFileFor(DateTime.Now);
                    if (!string.Equals(_logPath, current, StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.CreateDirectory(PluginPaths.LogsDirectory);
                        _logPath = current;
                    }

                    WriteLineUnsafe(level, message);
                }
            }
            catch
            {
                // A logging failure is intentionally swallowed at the plugin boundary.
            }
        }

        private static void WriteLineUnsafe(string level, string message)
        {
            var line = string.Format(
                "{0:O} {1,-5} v{2} [{3}] {4}{5}",
                DateTime.Now,
                level,
                VersionInfo.ProductVersion,
                "Infrastructure",
                message ?? string.Empty,
                Environment.NewLine);
            File.AppendAllText(_logPath, line, LogEncoding);
        }
    }
}
