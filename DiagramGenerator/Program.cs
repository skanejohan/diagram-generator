using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiagramGenerator
{
    public class Program
    {
        private static CSharpAnalyzer Analyzer;
        private static CSharpObjectCollection Collection;
        private static Settings Settings;

        public static void Main(string[] args)
        {
            string dir;
            if (args.Length == 0)
            {
                dir = Directory.GetCurrentDirectory();
                Console.WriteLine($"(assuming current directory {dir})");
            }
            else
            {
                dir = args[0];
                Console.WriteLine($"Processing directory {dir}");
            }

            Analyzer = new CSharpAnalyzer(Console.WriteLine);
            IEnumerable<string> files = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.cs", SearchOption.AllDirectories);
            Collection = Analyzer.AnalyzeFiles(files);

            List<string> settingsFiles = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.dg.cfg", SearchOption.AllDirectories).ToList();
            if (settingsFiles.Count == 0)
            {
                settingsFiles.Add(dir + @"\default.dg.cfg");
            }

            foreach (string settingsFile in settingsFiles)
            {
                Settings = new Settings(settingsFile);
                CSharpObjectCollection coll = Settings.StartClass == "" ? Collection : Collection.Clone(Settings.StartClass, Settings, int.MaxValue);
                SaveFile(UmlGenerator.GeneratePlantUml(coll), settingsFile.Replace(".dg.cfg", ".plantuml"));
            }
        }

        private static void SaveFile(List<string> uml, string fileName)
        {
            using TextWriter tw = new StreamWriter(fileName);
            foreach (string s in uml)
            {
                tw.WriteLine(s);
            }
        }
    }
}