using GiacintFlasher.Lib.Data;
using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace GiacintFlasher.Lib.Services
{
    public class FDroidPackageInfo
    {
        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("suggestedVersionCode")]
        public int SuggestedVersionCode { get; set; }

        [JsonProperty("packages")]
        public Package[] Packages { get; set; }
    }

    public class Package
    {
        [JsonProperty("versionName")]
        public string VersionName { get; set; }

        [JsonProperty("versionCode")]
        public int VersionCode { get; set; }
    }
    internal class LV
    {
        internal static async Task InstallPackage(string packageName)
        {
            foreach (var source in Flasher.Config.LvSources)
            {
                var extension = source.IsXAPK ? ".xapk" : ".apk";
                var packageLink = source.Url.Replace("%com.package.name%", packageName);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (source.CleanLink == false)
                        {
                            var directLink = await GetDirectApkLinkAsync(packageLink);
                            await LibInstaller.DownloadFileAsync(directLink, Path.Combine(Environment.CurrentDirectory, packageName + extension));
                        }
                        else
                            await LibInstaller.DownloadFileAsync(packageLink, Path.Combine(Environment.CurrentDirectory, packageName + extension));
                    }
                    catch (Exception ex)
                    {
                        Debug.Error($"Error installing package from {source.Name}: {ex.Message}");
                        return;
                    }
                        
                    Debug.Success($"Package {packageName} installed successfully from {source.Name}.");
                        
                    return;
                });
            }

            Debug.Error($"Package {packageName} not found in any source.");
        }

        internal static async Task<FDroidPackageInfo> GetPackageInfoAsync(string packageId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://f-droid.org/api/v1/packages/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(packageId);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    FDroidPackageInfo packageInfo = JsonConvert.DeserializeObject<FDroidPackageInfo>(json);
                    return packageInfo;
                }
                else
                {
                    throw new Exception($"Ошибка API: {response.StatusCode}");
                }
            }
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

        internal static async Task<string?> GetDirectApkLinkAsync(string downloadPageUrl)
        {
            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            using var client = new HttpClient(handler);

            // APKPure требует User-Agent и Referer
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            client.DefaultRequestHeaders.Referrer = new Uri("https://apkpure.com/");

            try
            {
                var response = await client.GetAsync(downloadPageUrl);

                if (response.StatusCode == HttpStatusCode.Found ||
                    response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    var location = response.Headers.Location;
                    if (location != null)
                    {
                        string directUrl = location.IsAbsoluteUri
                            ? location.ToString()
                            : new Uri(new Uri(downloadPageUrl), location).ToString();

                        return directUrl;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("⚠️ 493.");
                }
                else
                {
                    Console.WriteLine($"⚠️ No redirect: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Invalid url: " + ex.Message);
            }

            return null;
        }

        internal static string GetFdroidPackageUrl(string packageName, int versionCode) => $"https://f-droid.org/repo/{packageName}_{versionCode}.apk";
    }
}
