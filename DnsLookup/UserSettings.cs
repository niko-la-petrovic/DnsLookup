using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DnsLookup
{
    public class UserSettings
    {
        private List<string> domainNames = new();

        public List<string> DomainNames
        {
            get => domainNames; set
            {
                domainNames = value;
            }
        }

        public bool CopyToClipboard { get; set; }

        [JsonIgnore]
        public static string SettingsFilePath => Path.Join(AppDataDir, SettingsFileName);

        [JsonIgnore]
        public static readonly string SettingsFileName = "appsettings.json";

        [JsonIgnore]
        public static string AppDataDir => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(DnsLookup));

        public async static Task<UserSettings> LoadAsync()
        {
            if (!File.Exists(SettingsFilePath))
            {
                UserSettings initialSettings = new();
                await initialSettings.SaveAsync();
            }

            using var fs = File.OpenRead(SettingsFilePath);
            var settings = await JsonSerializer.DeserializeAsync<UserSettings>(fs);

            return settings;
        }

        public async Task SaveAsync()
        {
            if (!File.Exists(AppDataDir))
                Directory.CreateDirectory(AppDataDir);

            string json = JsonSerializer.Serialize(this);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
    }
}
