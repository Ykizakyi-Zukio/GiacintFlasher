namespace GiacintFlasher.Lib.Services
{
    internal class LibInstaller
    {
        internal static async Task DownloadFileAsync(string url, string path)
        {
            try
            {
                Debug.Info($"Downloading from {url} to {path}...");
                using HttpClient client = new HttpClient();
                Debug.Info("Starting download...");

                // Получаем поток из интернета
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Debug.Info("Download in progress...");

                // Сохраняем в файл
                Debug.Info("Saving as file...");
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);
                Debug.Success("Download completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.Error("Error downloading file: " + ex.Message);
            }
        }
    }
}
