using System;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CADProjectManager.Core;
using CADProjectManager.Infrastructure;

namespace CADProjectManager.AutoCAD
{
    public sealed class PluginEntry : IExtensionApplication
    {
        public void Initialize()
        {
            try
            {
                // V0.1 deliberately performs no document, database, UI, network, event, or background work here.
                BootstrapLogger.Initialize();
                BootstrapLogger.Info("AutoCAD adapter initialized in " + VersionInfo.SafeMode + ".");
            }
            catch
            {
                // Initialization must never drag down AutoCAD.
            }
        }

        public void Terminate()
        {
            try
            {
                BootstrapLogger.Info("AutoCAD adapter terminating.");
                BootstrapLogger.Flush();
            }
            catch
            {
                // Termination must never block AutoCAD shutdown.
            }
        }

        [CommandMethod("CADPM_HEALTH", CommandFlags.Session)]
        public void Health()
        {
            GlobalExceptionBoundary.Execute("CADPM_HEALTH", editor =>
            {
                editor.WriteMessage("\nCAD Project Manager");
                editor.WriteMessage("\nStatus: OK");
                editor.WriteMessage("\nVersion: " + VersionInfo.ProductVersion);
                editor.WriteMessage("\nAutoCAD: " + SafeAutoCadVersion());
                editor.WriteMessage("\nRuntime: " + Environment.Version);
                editor.WriteMessage("\nMode: " + VersionInfo.SafeMode);
            });
        }

        [CommandMethod("CADPM_INFO", CommandFlags.Session)]
        public void Info()
        {
            GlobalExceptionBoundary.Execute("CADPM_INFO", editor =>
            {
                editor.WriteMessage("\nCAD Project Manager");
                editor.WriteMessage("\nPlugin version: " + VersionInfo.ProductVersion);
                editor.WriteMessage("\nBuild: " + GetBuildVersion());
                editor.WriteMessage("\nAutoCAD: " + SafeAutoCadVersion());
                editor.WriteMessage("\n.NET runtime: " + Environment.Version);
                editor.WriteMessage("\nPlugin path: " + GetPluginPath());
                editor.WriteMessage("\nConfig path: " + PluginPaths.ConfigDirectory);
                editor.WriteMessage("\nLog path: " + BootstrapLogger.LogPath);
            });
        }

        private static string SafeAutoCadVersion()
        {
            try
            {
                var version = Application.Version;
                return version == null ? "unavailable" : version.ToString();
            }
            catch (System.Exception exception)
            {
                BootstrapLogger.Error("Unable to read AutoCAD version.", exception);
                return "unavailable";
            }
        }

        private static string GetPluginPath()
        {
            try
            {
                return Assembly.GetExecutingAssembly().Location;
            }
            catch (System.Exception exception)
            {
                BootstrapLogger.Error("Unable to read plugin path.", exception);
                return "unavailable";
            }
        }

        private static string GetBuildVersion()
        {
            try
            {
                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                return assemblyVersion == null ? "unavailable" : assemblyVersion.ToString();
            }
            catch (System.Exception exception)
            {
                BootstrapLogger.Error("Unable to read build version.", exception);
                return "unavailable";
            }
        }
    }
}
