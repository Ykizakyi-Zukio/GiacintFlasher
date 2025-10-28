using GiacintFlasher.Lib.Data;

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

                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;
                await using var contentStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                Debug.Info("Download started...\r\n");
                await CopyToAsyncWithProgress(contentStream, fileStream, totalBytes);

                Console.WriteLine("\r\n");
                Debug.Success("Download completed successfully.");
                Flasher.WelcomeMessage();
                Console.CursorVisible = true;

                contentStream.Close();  
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Debug.Error("Error downloading file: " + ex.Message);
            }
        }

        private static async Task CopyToAsyncWithProgress(Stream source, Stream destination, long? totalBytes)
        {
            byte[] buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;
            int lastPercent = -1;

            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
                totalRead += bytesRead;

                if (totalBytes.HasValue)
                {
                    int percent = (int)((totalRead * 100L) / totalBytes.Value);
                    if (percent != lastPercent)
                    {
                        DrawProgressBar(percent);
                        lastPercent = percent;
                    }
                }
            }
        }

        private static void DrawProgressBar(int percent)
        {
            Console.Clear();
            const int barSize = 40;
            int filled = (int)(barSize * percent / 100.0);

            Console.CursorVisible = false;
            //Console.Write("\r\n[");
            Console.WriteLine(Color.Success);
            Console.Write(new string('■', filled));
            Console.Write(Color.Reset);
            Console.Write(new string('■', barSize - filled));
            //Console.Write($"] {percent,3}%");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
