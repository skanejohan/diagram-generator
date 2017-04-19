using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DiagramGenerator
{
    public class Program
    {
        private static CSharpAnalyzer Analyzer;
        private static UmlGenerator Generator;
        private static CSharpObjectCollection Collection;
        private static Settings Settings;

        private static void SaveFile(List<string> uml, string fileName)
        {
            using (TextWriter tw = new StreamWriter(fileName))
            {
                foreach (String s in uml)
                {
                    tw.WriteLine(s);
                }
            }
        }

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
            Generator = new UmlGenerator();
            var files = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.cs", SearchOption.AllDirectories);
            Collection = Analyzer.AnalyzeFiles(files);

            var settingsFiles = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.dg.cfg", SearchOption.AllDirectories).ToList();
            if (settingsFiles.Count == 0)
            {
                settingsFiles.Add(dir + @"\default.dg.cfg");
            }

            foreach (var settingsFile in settingsFiles)
            {
                Settings = new Settings(settingsFile);
                var coll = Settings.StartClass == "" ? Collection : Collection.Clone(Settings.StartClass, Settings, Int32.MaxValue);
                SaveFile(Generator.GeneratePlantUml(coll), settingsFile.Replace(".dg.cfg", ".plantuml"));
            }
        }
    }
}
