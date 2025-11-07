using GiacintFlasher.Lib.Data;
using System.Reflection.Metadata.Ecma335;

namespace GiacintFlasher.Lib.Services
{
    internal class LV
    {
        internal static async Task InstallPackage(string packageName)
        {
            foreach (var source in Flasher.Config.LvSources)
            {
                var extension = source.IsXAPK ? ".xapk" : ".apk";
                var packageLink = source.Url.Replace("%com.package.name%", packageName);

                //if (!await UrlExistsAsync(packageLink))
                //{
                //    Debug.Error(packageLink);
                //    continue;
                //}

                //if (!source.CleanLink)
                //{
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (source.CleanLink == false)
                            {
                                var directLink = await GetDirectApkLinkAsync(packageLink);
                                await LibInstaller.DownloadFileAsync(directLink, Path.Combine(Environment.CurrentDirectory, packageName + extension));
                            }
                                //await LibInstaller.InstallDynamic(packageLink, Path.Combine(Environment.CurrentDirectory, packageName + extension));
                            else
                                await LibInstaller.DownloadFileAsync(packageLink, Path.Combine(Environment.CurrentDirectory, packageName + extension));
                        }
                        catch (Exception ex)
                        {
                            Debug.Error($"Error installing package from {source.Name}: {ex.Message}");
                            return;
                        }
                        //finally
                        //{
                            Debug.Success($"Package {packageName} installed successfully from {source.Name}.");
                        //}
                        return;
                    });
                //}
            }

            Debug.Error($"Package {packageName} not found in any source.");
        }

        internal static async Task<bool> UrlExistsAsync(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(Flasher.Config.WebTimeout);

                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                }

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<string> GetDirectApkLinkAsync(string dynamicUrl)
        {
            try
            {
                // Настраиваем HttpClient для не следования редиректам автоматически
                using (var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false
                })
                using (var tempClient = new HttpClient(handler))
                {
                    // Делаем HEAD запрос для получения финального URL без загрузки файла
                    var response = await tempClient.GetAsync(dynamicUrl, HttpCompletionOption.ResponseHeadersRead);

                    // Проверяем статус редиректа (301, 302, 303, 307, 308)
                    if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                    {
                        // Получаем финальный URL из заголовка Location
                        var finalUrl = response.Headers.Location?.ToString();

                        if (!string.IsNullOrEmpty(finalUrl))
                        {
                            // Если URL относительный, делаем его абсолютным
                            if (!finalUrl.StartsWith("http"))
                            {
                                var baseUri = new Uri(dynamicUrl);
                                finalUrl = new Uri(baseUri, finalUrl).ToString();
                            }

                            return finalUrl;
                        }
                    }

                    // Если редиректа нет, возвращаем исходный URL
                    return dynamicUrl;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении прямой ссылки: {ex.Message}", ex);
            }
        }
    }
}
