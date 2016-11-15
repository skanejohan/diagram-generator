using System;
using System.IO;
using System.Collections.Generic;

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

            Settings = new Settings(dir + @"\dg.cfg");
            Analyzer = new CSharpAnalyzer(Settings, Console.WriteLine);
            Generator = new UmlGenerator();
            var files = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.cs", SearchOption.AllDirectories);
            Collection = Analyzer.AnalyzeFiles(files);
            SaveFile(Generator.GeneratePlantUml(Collection), $"{dir}\\uml.plantuml");
        }
    }
}
