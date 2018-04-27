using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Task = System.Threading.Tasks.Task;

namespace VsixSynchronizer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]       
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideCodeGenerator(typeof(VsctCodeGenerator), VsctCodeGenerator.Name, VsctCodeGenerator.Description, true)]
    [ProvideCodeGenerator(typeof(VsixManifestCodeGenerator), VsixManifestCodeGenerator.Name, VsixManifestCodeGenerator.Description, true)]
    [Guid(PackageGuids.guidPackageString)]
    [ProvideUIContextRule(PackageGuids.guidUIContextString,
        name: "UI Context",
        expression: "vsct | vsixmanifest",
        termNames: new[] { "vsct", "vsixmanifest" },
        termValues: new[] { "HierSingleSelectionName:.vsct$", "HierSingleSelectionName:.vsixmanifest$" })]
    public sealed class VsixSynchronizerPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ToggleGeneratorSync.InitializeAsync(this);
        }
    }
}
