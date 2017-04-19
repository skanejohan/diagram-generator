using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;

namespace CSharpSyntaxWalkerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = new FileStream(@"C:\Users\JAH\Documents\Git\xgsos[2]\Kernel\XgsOS.Hardware\XgsOS.Hardware.Factory\HardwareFactory.cs", FileMode.Open, FileAccess.Read))
            {
                var tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(SourceText.From(stream));
                new Walker().Visit(tree.GetRoot());
                Console.ReadLine();
            }
        }
    }

    public class Walker : CSharpSyntaxWalker
    {
        static int Tabs = 0;

        public Walker() : base(SyntaxWalkerDepth.Token)
        {
        }

        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new string('\t', Tabs);
            Console.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }

        public override void VisitToken(SyntaxToken token)
        {
            var indents = new string('\t', Tabs);
            Console.WriteLine(indents + token);
            base.VisitToken(token);
        }
    }
}
