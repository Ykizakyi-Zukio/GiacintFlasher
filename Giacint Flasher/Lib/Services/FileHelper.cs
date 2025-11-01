namespace GiacintFlasher.Lib.Services
{
    internal class FileHelper
    {
        internal static async Task MoveAll(string sourceDir, string destDir, bool deleteSourceDir = true)
        {
            string[] files = Directory.GetFiles(sourceDir);

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(destDir, fileName);
                await Task.Run(() => File.Move(file, destPath, true));
            }

            if (deleteSourceDir)
                Directory.Delete(sourceDir, true);
        }
    }
}
