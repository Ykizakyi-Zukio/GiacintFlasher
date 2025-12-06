using System.Diagnostics;

namespace GiacintFlasher.Lib.Services
{
    internal static class SF
    {
        internal static void OperandAppList(string cmd, string[] apps)
        {
            Debug.Info($"Staring {cmd} for app list {string.Join(", ", apps)}");
            foreach (var app in apps)
            {
                try
                {
                    Flasher.Command($"lib adb shell pm {cmd} {app}");
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
