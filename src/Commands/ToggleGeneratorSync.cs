using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using System;
using Task = System.Threading.Tasks.Task;

namespace VsixSynchronizer
{
    internal sealed class ToggleGeneratorSync
    {
        private static readonly Dictionary<string, string> _generators = new Dictionary<string, string>
        {
            { ".vsct", VsctGenerator.Name },
            { ".vsixmanifest", VsixManifestGenerator.Name }
        };

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(dte);

            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as IMenuCommandService;
            Assumes.Present(commandService);

            var cmdId = new CommandID(new Guid("c6562423-a610-432a-8efa-ca46df10ddd6"), 0x0200);

            var cmd = new OleMenuCommand((s, e) => { OnExecute(dte); }, cmdId)
            {
                // This will defer visibility control to the VisibilityConstraints section in the .vsct file
                Supported = false
            };

            commandService.AddCommand(cmd);
        }

        private static void OnExecute(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ProjectItem item = dte.SelectedItems.Item(1).ProjectItem;
            var ext = Path.GetExtension(item?.FileNames[1] ?? "")?.ToLowerInvariant();

            if (_generators.TryGetValue(ext, out var generator))
            {
                item.Properties.Item("CustomTool").Value = generator;
            }
        }
    }
}
