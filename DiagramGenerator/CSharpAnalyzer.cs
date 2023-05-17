using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiagramGenerator
{
    public class CSharpAnalyzer
    {
        private readonly Action<string> log;

        public CSharpAnalyzer(Action<string> log = null)
        {
            this.log = log;
        }

        public async Task<CSharpObjectCollection> AnalyzeFilesAsync(IEnumerable<string> files)
        {
            return await Task.Run(() => AnalyzeFiles(files));
        }

        public CSharpObjectCollection AnalyzeFiles(IEnumerable<string> files)
        {
            CSharpVisitor visitor = new();
            CSharpObjectCollection coll = new();

            // Pass 1 - add all classes and interfaces found in the files.
            visitor.HandleClass = c =>
            {
                string name = c.Identifier.ToString();
                coll.AddClass(name);
                log?.Invoke($"Class: {name}");
            };
            visitor.HandleInterface = i =>
            {
                string name = i.Identifier.ToString();
                coll.AddInterface(name);
                log?.Invoke($"Interface: {name}");
            };
            foreach (string file in files)
            {
                visitor.Visit(file);
            }

            // Pass 2 - handle extension and composition for all classes found in the files.
            visitor.HandleClass = c => DetermineExtensionAndComposition(c, coll);
            visitor.HandleInterface = null;
            foreach (string file in files)
            {
                visitor.Visit(file);
            }

            return coll;
        }

        private static CSharpVisibility GetVisibility(FieldDeclarationSyntax field)
        {
            foreach (Microsoft.CodeAnalysis.SyntaxToken modifier in field.Modifiers)
            {
                if (modifier.Text == "private")
                {
                    return CSharpVisibility.Private;
                }
                if (modifier.Text == "protected")
                {
                    return CSharpVisibility.Protected;
                }
                if (modifier.Text == "internal")
                {
                    return CSharpVisibility.Internal;
                }
                if (modifier.Text == "public")
                {
                    return CSharpVisibility.Public;
                }
            }
            return CSharpVisibility.Public;
        }

        private void DetermineExtensionAndComposition(ClassDeclarationSyntax c, CSharpObjectCollection coll)
        {
            if (c.BaseList != null)
            {
                foreach (BaseTypeSyntax baseType in c.BaseList.Types)
                {
                    coll.SetExtends(c.Identifier.ToString(), baseType.Type.ToString());
                    log?.Invoke($"{c.Identifier} extends {baseType.Type}");
                }
            }
            foreach (MemberDeclarationSyntax member in c.Members)
            {
                if (member is FieldDeclarationSyntax)
                {
                    try
                    {
                        CSharpVisibility visibility = GetVisibility(member as FieldDeclarationSyntax);
                        FieldDeclarationSyntax field = member as FieldDeclarationSyntax;
                        TypeSyntax type = field.Declaration.Type;
                        if (type is IdentifierNameSyntax)
                        {
                            coll.SetAssociation(
                                c.Identifier.ToString(),
                                (type as IdentifierNameSyntax).Identifier.Text,
                                visibility);
                            log?.Invoke(
                                $"{c.Identifier} associates to {(type as IdentifierNameSyntax).Identifier.Text}");
                        }
                        else if (type is GenericNameSyntax)
                        {
                            coll.SetAssociation(
                                c.Identifier.ToString(),
                                (type as GenericNameSyntax).Identifier.Text,
                                visibility);
                            foreach (TypeSyntax arg in (type as GenericNameSyntax).TypeArgumentList.Arguments)
                            {
                                coll.SetAssociation(c.Identifier.ToString(), arg.ToString(), visibility);
                                log?.Invoke($"{c.Identifier} associates to {arg}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log?.Invoke(e.ToString());
                    }
                }
            }
        }
    }
}