using Microsoft.VisualStudio.Setup.Configuration;

namespace VsixSynchronizer
{
    public static class VsVersionHelper
    {
        public static bool IsVs2019()
        {
            ISetupConfiguration configuration = new SetupConfiguration();
            ISetupInstance instance = configuration.GetInstanceForCurrentProcess();
            var _vsProductVersion = instance.GetInstallationVersion();

            return _vsProductVersion.StartsWith("16.");
        }
    }
}
