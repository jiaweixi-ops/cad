using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using CADProjectManager.Infrastructure;

namespace CADProjectManager.AutoCAD
{
    internal static class GlobalExceptionBoundary
    {
        public static void Execute(string commandName, Action<Editor> action)
        {
            try
            {
                var document = Application.DocumentManager.MdiActiveDocument;
                var editor = document == null ? null : document.Editor;

                if (editor == null)
                {
                    BootstrapLogger.Info(commandName + " skipped: no active document.");
                    return;
                }

                action(editor);
            }
            catch (Exception exception)
            {
                try
                {
                    BootstrapLogger.Error(commandName + " failed.", exception);
                }
                catch
                {
                    // Logging failures must never escape the command boundary.
                }

                try
                {
                    var document = Application.DocumentManager.MdiActiveDocument;
                    var editor = document == null ? null : document.Editor;
                    if (editor != null)
                    {
                        editor.WriteMessage("\nCAD Project Manager: command failed safely. See the plugin log for details.");
                    }
                }
                catch
                {
                    // Never allow recovery/reporting code to leak an exception into AutoCAD.
                }
            }
        }
    }
}
