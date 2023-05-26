using System;
using System.Collections.Generic;
using System.IO;

namespace DiagramGenerator
{
    public class ConfigFile
    {
        private readonly Dictionary<string, string> values;

        public ConfigFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            values = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                int eqPos = line.IndexOf("=");
                if (eqPos > 0)
                {
                    values.Add(line[..eqPos].Trim(), line[(eqPos + 1)..].Trim());
                }
            }
        }

        public string ReadString(string key, string def = "")
        {
            return Read(key, s => s, def);
        }

        public bool ReadBool(string key, bool def = false)
        {
            return Read(key, s => s.ToLower() == "1" || s.ToLower() == "t" || s.ToLower() == "true", def);
        }

        private T Read<T>(string key, Func<string, T> converter, T def)
        {
            if (!values.TryGetValue(key, out string s))
            {
                return def;
            }
            return converter(s);
        }
    }
}