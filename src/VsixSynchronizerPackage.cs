using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VsixSynchronizer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("VSIX Synchronizer", @"Provides the ability to generate code-behind files for .vsixmanfiest and .vsct files in managed code to make the information easy to consume from the rest of the extension.", "1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]

    [ProvideCodeGenerator(typeof(VsctGenerator), VsctGenerator.Name, VsctGenerator.Description, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid, RegisterCodeBase = true)]
    [ProvideCodeGeneratorExtension(VsctGenerator.Name, ".vsct")]
    
    [ProvideCodeGenerator(typeof(VsixManifestGenerator), VsixManifestGenerator.Name, VsixManifestGenerator.Description, true, ProjectSystem = ProvideCodeGeneratorAttribute.CSharpProjectGuid, RegisterCodeBase = true)]
    [ProvideCodeGeneratorExtension(VsixManifestGenerator.Name, ".vsixmanifest")]

    [Guid("8c737258-7da1-4314-97e9-cb61f7cf8d22")]
    [ProvideUIContextRule("f19443c0-4f6b-45c3-bea5-80c1f8a538dd",
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
