using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;

namespace DiagramGenerator
{
    public class CSharpVisitor
    {
        private readonly Walker walker;

        public CSharpVisitor()
        {
            walker = new Walker();
        }

        public Action<ClassDeclarationSyntax> HandleClass
        {
            get { return walker.handleClass; }
            set { walker.handleClass = value; }
        }

        public Action<InterfaceDeclarationSyntax> HandleInterface
        {
            get { return walker.handleInterface; }
            set { walker.handleInterface = value; }
        }

        public void Visit(CSharpSyntaxTree tree)
        {
            walker.Visit(tree.GetRoot());
        }

        public void Visit(string fileName)
        {
            using FileStream stream = new(fileName, FileMode.Open, FileAccess.Read);
            CSharpSyntaxTree tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(SourceText.From(stream));
            Visit(tree);
        }

        private class Walker : CSharpSyntaxWalker
        {
            public Action<ClassDeclarationSyntax> handleClass;
            public Action<InterfaceDeclarationSyntax> handleInterface;

            public override void Visit(SyntaxNode node)
            {
                if (node is ClassDeclarationSyntax syntax)
                {
                    handleClass?.Invoke(syntax);
                }
                if (node is InterfaceDeclarationSyntax syntax1)
                {
                    handleInterface?.Invoke(syntax1);
                }
                base.Visit(node);
            }
        }
    }
}