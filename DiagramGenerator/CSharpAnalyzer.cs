using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiagramGenerator
{
    public class CSharpAnalyzer
    {
        private readonly Action<string> log;

        public CSharpAnalyzer(Action<string> log = null)
        {
            this.log = log;
        }

        public CSharpObjectCollection AnalyzeFiles(IEnumerable<string> files)
        {
            var visitor = new CSharpVisitor();
            var coll = new CSharpObjectCollection();

            // Pass 1 - add all classes and interfaces found in the files.
            visitor.HandleClass = c =>
            {
                var name = c.Identifier.ToString();
                coll.AddClass(name);
                log?.Invoke($"Class: {name}");
            };
            visitor.HandleInterface = i =>
            {
                var name = i.Identifier.ToString();
                coll.AddInterface(name);
                log?.Invoke($"Interface: {name}");
            };
            foreach (var file in files)
            {
                visitor.Visit(file);
            }

            // Pass 2 - handle extension and composition for all classes found in the files.
            visitor.HandleClass = c => DetermineExtensionAndComposition(c, coll);
            visitor.HandleInterface = null;
            foreach (var file in files)
            {
                visitor.Visit(file);
            }

            return coll;
        }

        private CSharpVisibility GetVisibility(FieldDeclarationSyntax field)
        {
            foreach (var modifier in field.Modifiers)
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
                foreach (var baseType in c.BaseList.Types)
                {
                    coll.SetExtends(c.Identifier.ToString(), baseType.Type.ToString());
                    log?.Invoke($"{c.Identifier.ToString()} extends {baseType.Type.ToString()}");
                }
            }
            foreach (var member in c.Members)
            {
                if (member is FieldDeclarationSyntax)
                {
                    try
                    {
                        var visibility = GetVisibility(member as FieldDeclarationSyntax);
                        var field = member as FieldDeclarationSyntax;
                        var type = field.Declaration.Type;
                        if (type is IdentifierNameSyntax)
                        {
                            coll.SetAssociation(
                                c.Identifier.ToString(),
                                (type as IdentifierNameSyntax).Identifier.Text,
                                visibility);
                            log?.Invoke(
                                $"{c.Identifier.ToString()} associates to {(type as IdentifierNameSyntax).Identifier.Text}");
                        }
                        else if (type is GenericNameSyntax)
                        {
                            coll.SetAssociation(
                                c.Identifier.ToString(), 
                                (type as GenericNameSyntax).Identifier.Text,
                                visibility);
                            foreach (var arg in (type as GenericNameSyntax).TypeArgumentList.Arguments)
                            {
                                coll.SetAssociation(c.Identifier.ToString(), arg.ToString(), visibility);
                                log?.Invoke($"{c.Identifier.ToString()} associates to {arg.ToString()}");
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
