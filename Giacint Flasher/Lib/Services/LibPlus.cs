using System.Runtime.InteropServices;

namespace GiacintFlasher.Lib.Services
{
    internal static class LibPlus
    {
        internal static string? FindLib(string libName)
        {
            string[] allFiles = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                allFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.exe", SearchOption.AllDirectories);
            else
                allFiles = Directory.GetFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                if (Path.GetFileNameWithoutExtension(file).Equals(libName, StringComparison.OrdinalIgnoreCase))
                    return file;
            }

            return null;
        }

        internal static async Task TryRunLib(string libName, string args)
        {
            try
            {
                var libPath = FindLib(libName);
                if (libPath == null) {Debug.Warning("This library not exists"); return; }
                string result = await ProcessHelper.RunCommandAsync(libPath, args);
                Debug.Info(result);
            }
            catch (Exception ex)
            {
                Debug.Error($"Error executing lib {libName}: {ex.Message}");
            }
        }
    }
}
