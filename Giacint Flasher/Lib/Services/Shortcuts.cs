namespace GiacintFlasher.Lib.Services
{
    internal class Shortcuts
    {
        internal static void SaveShortcut(string name, string command)
        {
            File.WriteAllText(Environment.CurrentDirectory + $"\\shortcuts\\{name}.shortcut", command);
        }

        internal static void InitShortcut(string name)
        {
            string path = Environment.CurrentDirectory + $"\\shortcuts\\{name}.shortcut";
            if (!File.Exists(path))
            {
                Debug.Error($"Shortcut '{name}' does not exist.");
                return;
            }
            string command = File.ReadAllText(path);
            Flasher.Command(command);
        }
    }
}
