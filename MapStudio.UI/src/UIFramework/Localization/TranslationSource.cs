using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using Toolbox.Core;

namespace MapStudio.UI
{
    /// <summary>
    /// Translation handler for translating given keys into localized text.
    /// </summary>
    public class TranslationSource : INotifyPropertyChanged
    {
        private static readonly TranslationSource instance = new TranslationSource();

        private Dictionary<string, string> Translation = new Dictionary<string, string>();

        /// <summary>
        /// The language key to determine what language file to use.
        /// </summary>
        public static string LanguageKey = "English";

        /// <summary>
        /// The instance of the translation source.
        /// </summary>
        public static TranslationSource Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Checks if the translation lookup has a given key or not.
        /// </summary>
        public static bool HasKey(string key) {
            if (key == null) return false;

            return Instance.Translation.ContainsKey(key);
        }

        /// <summary>
        /// Returns the localized text from a given key.
        /// </summary>
        public static string GetText(string key) {
            return Instance[key];
        }

        /// <summary>
        /// Returns the localized text from a given key.
        /// </summary>
        public string this[string key]
        {
            get 
            {
                //Return the key by itself if not contained in the translation list.
                if (!HasKey(key))
                    return key;

                return Translation[key];
            }
        }

        /// <summary>
        /// Gets a list of language file paths in the "Languages" folder.
        /// </summary>
        public static List<string> GetLanguages()
        {
            List<string> languages = new List<string>();
            foreach (var file in Directory.GetDirectories(Path.Combine("Lib","Languages"))) {
                languages.Add(new DirectoryInfo(file).Name);
            }
            return languages;
        }

        /// <summary>
        /// Updates the current language and reloads the translation list.
        /// </summary>
        public void Update(string key)
        {
            LanguageKey = key;
            Reload();
        }

        public void DumpUntranslated()
        {
            if (LanguageKey == "English")
                return;
            foreach (var src in Directory.GetFiles(Path.Combine(Runtime.ExecutableDir,"Lib","Languages","English")))
            {
                string dest = src.Replace("Lib/Languages/English", $"Lib/Languages/{LanguageKey}");
                DumpUntranslated(src, dest);
            }
        }

        public void DumpUntranslated(string src, string dest)
        {
            Dictionary<string, string> original = new Dictionary<string, string>();
            Dictionary<string, string> translated = new Dictionary<string, string>();

            Load(src, original);
            Load(dest, translated);

            using (var writer = new StreamWriter(File.OpenWrite($"{dest}_untranslated.txt")))
            {
                foreach (var keyPair in original) {
                    //Check if target translation has a proper translation entry.
                    if (!translated.ContainsKey(keyPair.Key) || string.IsNullOrEmpty(keyPair.Value)) {
                        writer.WriteLine($"{keyPair.Key} -");
                    }
                }
            }
        }

        /// <summary>
        // Reloads the translation list with the current language key.
        /// </summary>
        public void Reload() {
            Translation = Load(LanguageKey);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        static Dictionary<string, string> Load(string folder)
        {
            Dictionary<string, string> translated = new Dictionary<string, string>(); ;
            foreach (var file in Directory.GetFiles(Path.Combine(Runtime.ExecutableDir,"Lib","Languages",folder)))
                Load(file, translated);
            return translated;
        }

        /// <summary>
        // Loads the language file and adds the keys with localized text to the translation lookup.
        /// </summary>
        static void Load(string fileName, Dictionary<string, string> translated)
        {
            if (!File.Exists(fileName))
                return;

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.StartsWith("#") || !line.Contains("-"))
                        continue;

                    var entries = line.Split('-');
                    //Remove comments at end if used
                    var value = entries[1].Split('#').FirstOrDefault().Trim();
                    var key = entries[0].Trim();
                    //Skip empty untranslated values
                    if (string.IsNullOrEmpty(value))
                        continue;

                    if (!translated.ContainsKey(key))
                        translated.Add(key, value);
                    else
                        translated[key] = value;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
