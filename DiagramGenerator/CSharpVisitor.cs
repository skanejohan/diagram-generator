using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

public class CSharpVisitor
{
	private class Walker : CSharpSyntaxWalker
	{
        public Action<ClassDeclarationSyntax> handleClass;
        public Action<InterfaceDeclarationSyntax> handleInterface;

        public override void Visit(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax)
            {
                handleClass?.Invoke((ClassDeclarationSyntax)node);
            }
            if (node is InterfaceDeclarationSyntax)
            {
                handleInterface?.Invoke((InterfaceDeclarationSyntax)node);
            }
            base.Visit(node);
        }
    }

    private Walker walker;

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

    public CSharpVisitor()
	{
		walker = new Walker();
	}

	public void Visit(CSharpSyntaxTree tree)
	{
		walker.Visit(tree.GetRoot());
	}

    public void Visit(string fileName)
    {
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            var tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(SourceText.From(stream));
            Visit(tree);
        }
    }
}