using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VsixSynchronizer
{
    internal sealed class ToggleGeneratorSync
    {
        private static DTE2 _dte;
        private static Dictionary<string, string> _generators = new Dictionary<string, string>
        {
            { ".vsct", VsctCodeGenerator.Name },
            { ".vsixmanifest", VsixManifestCodeGenerator.Name }
        };

        public static async Task InitializeAsync(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;

            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as IMenuCommandService;
            var cmdId = new CommandID(PackageGuids.guidVsixSynchronizerCmdSet, PackageIds.ToggleVsctSyncId);

            var cmd = new OleMenuCommand(OnExecute, cmdId)
            {
                // This will defer visibility control to the VisibilityConstraints section in the .vsct file
                Supported = false
            };

            commandService.AddCommand(cmd);
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ProjectItem item = _dte.SelectedItems.Item(1).ProjectItem;
            string ext = Path.GetExtension(item?.FileNames[1] ?? "")?.ToLowerInvariant();

            if (_generators.TryGetValue(ext, out string generator))
            {
                item.Properties.Item("CustomTool").Value = generator;
            }
        }
    }
}
