using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;

namespace CSharpSyntaxWalkerTest
{
    public class Walker : CSharpSyntaxWalker
    {
        private static int Tabs = 0;

        public Walker() : base(SyntaxWalkerDepth.Token)
        {
        }

        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            string indents = new string('\t', Tabs);
            Console.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }

        public override void VisitToken(SyntaxToken token)
        {
            string indents = new string('\t', Tabs);
            Console.WriteLine(indents + token);
            base.VisitToken(token);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (FileStream stream = new FileStream(@"C:\Users\JAH\Documents\Git\xgsos[2]\Kernel\XgsOS.Hardware\XgsOS.Hardware.Factory\HardwareFactory.cs", FileMode.Open, FileAccess.Read))
            {
                CSharpSyntaxTree tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(SourceText.From(stream));
                new Walker().Visit(tree.GetRoot());
                Console.ReadLine();
            }
        }
    }
}