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
            var document = Application.DocumentManager.MdiActiveDocument;
            var editor = document == null ? null : document.Editor;

            try
            {
                if (editor == null)
                {
                    BootstrapLogger.Info(commandName + " skipped: no active document.");
                    return;
                }

                action(editor);
            }
            catch (Exception exception)
            {
                BootstrapLogger.Error(commandName + " failed.", exception);
                try
                {
                    editor.WriteMessage("\nCAD Project Manager: command failed safely. See the plugin log for details.");
                }
                catch
                {
                    // The exception boundary must not leak into AutoCAD.
                }
            }
        }
    }
}
