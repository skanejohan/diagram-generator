using System;
using System.IO;
using System.Collections.Generic;

namespace DiagramGenerator
{
    public class ConfigFile
    {
        private readonly Dictionary<string, string> values;

        public ConfigFile(string path)
        {
            var lines = File.ReadAllLines(path);
            values = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var eqPos = line.IndexOf("=");
                if (eqPos > 0)
                {
                    values.Add(line.Substring(0, eqPos).Trim(), line.Substring(eqPos+1).Trim());
                }
            }
        }

        private T Read<T>(string key, Func<string,T> converter, T def)
        {
            string s;
            if (!values.TryGetValue(key, out s))
            {
                return def;
            }
            return converter(s);
        }

        public string ReadString(string key, string def = "")
        {
            return Read(key, s => s, def);
        }

        public bool ReadBool(string key, bool def = false)
        {
            return Read(key, s => s.ToLower() == "1" || s.ToLower() == "t" || s.ToLower() == "true", def);    
        }

    }
}
