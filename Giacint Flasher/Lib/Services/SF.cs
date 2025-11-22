using System.Diagnostics;

namespace GiacintFlasher.Lib.Services
{
    internal static class SF
    {
        internal static void RemoveUserApps(string cmd, string[] apps)
        {
            Debug.Info($"Staring {cmd} for app list {string.Join(", ", apps)}");
            foreach (var app in apps)
            {
                try
                {
                    Flasher.Command($"lib adb shell pm {cmd} --user 0 {app}");
                    Debug.Success(app);
                }
                catch (Exception e)
                {
                    Debug.Error(app);
                    if (Flasher.Config.FullLogging)
                        Debug.Error(e);
                }
            }
        }
    }
}
