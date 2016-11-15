using System;
using System.IO;
using System.Collections.Generic;

namespace DiagramGenerator
{

    public class Program
    {
        private static readonly CSharpAnalyzer Analyzer = new CSharpAnalyzer(Console.WriteLine);
        private static readonly UmlGenerator Generator = new UmlGenerator();
        private static CSharpObjectCollection coll = null;

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

            var files = Directory.EnumerateFiles(Path.GetFullPath(dir), "*.cs", SearchOption.AllDirectories);
            coll = Analyzer.AnalyzeFiles(files);
            SaveFile(Generator.GeneratePlantUml(coll), $"{dir}\\uml.plantuml");
            Console.ReadLine();
        }
    }
}
