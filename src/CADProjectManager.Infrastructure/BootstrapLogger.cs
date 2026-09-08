using System;
using System.IO;
using System.Text;
using CADProjectManager.Core;

namespace CADProjectManager.Infrastructure
{
    public static class BootstrapLogger
    {
        private static readonly object Sync = new object();
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
            var details = exception == null ? message : message + " | " + exception;
            Write("ERROR", details);
        }

        public static void Flush()
        {
            // Each record is appended and closed synchronously. There is no background writer to flush.
        }

        private static void Write(string level, string message)
        {
            try
            {
                lock (Sync)
                {
                    if (!_initialized || string.IsNullOrEmpty(_logPath))
                    {
                        return;
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
            File.AppendAllText(_logPath, line, Encoding.UTF8);
        }
    }
}
