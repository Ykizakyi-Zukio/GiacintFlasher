using System.Text.Json.Serialization;

namespace GiacintFlasher.Lib.Data
{
    public struct Source
    {
        [JsonInclude]
        public string Name;
        [JsonInclude]
        public string Url;
        [JsonInclude]
        public bool IsXAPK;
        [JsonInclude]
        public bool CleanLink;

        internal Source(string name, string url, bool isXAPK = false, bool cleanLink = false)
        {
            Name = name;
            Url = url;
            IsXAPK = isXAPK;
            CleanLink = cleanLink;
        }
    }
}
