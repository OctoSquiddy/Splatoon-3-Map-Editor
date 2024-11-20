using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Toolbox.Core;

namespace MapStudio.UI
{
    public class AssetConfig
    {
        public float IconSize = 70.0f;

        public Dictionary<string, AssetSettings> Settings = new Dictionary<string, AssetSettings>();

        public AssetConfig() { }

        public void AddToFavorites(AssetItem item, int mnum)
        {
            if (!Settings.ContainsKey(item.ID))
                Settings.Add(item.ID, new AssetSettings());

            if (!item.Favoriteds[mnum - 1])
            {
                if (item.Categories.Contains($"Favourite {mnum}"))
                    item.Categories.Remove($"Favourite {mnum}");
            }
            else
            {
                if (!item.Categories.Contains($"Favourite {mnum}"))
                    item.Categories.Add($"Favourite {mnum}");
            }

            Settings[item.ID].Categories = item.Categories.ToArray();
            this.Save();
        }

        public void ApplySettings(AssetItem item, int mnum)
        {
            if (!Settings.ContainsKey(item.ID))
                return;

            var settings = Settings[item.ID];
            item.Categories = settings.Categories.ToList();
            if (item.Categories.Contains($"Favourite {mnum}"))
                item.Favoriteds[mnum - 1] = true;
        }

        public static AssetConfig Load()
        {
            string path = Path.Combine(Runtime.ExecutableDir,"Lib", $"AssetConfig.json");
            if (!File.Exists(path))
                new AssetConfig().Save();

            return JsonConvert.DeserializeObject<AssetConfig>(File.ReadAllText(path));
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path.Combine(Runtime.ExecutableDir,"Lib", $"AssetConfig.json"), json);
        }

        public class AssetSettings
        {
            public string[] Categories { get; set; } = new string[0];
        }
    }
}
