﻿using System;
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
                coll.AddClass(c.Identifier.ToString());
                log?.Invoke($"Class: {c.Identifier.ToString()}");
            };
            visitor.HandleInterface = i =>
            {
                coll.AddInterface(i.Identifier.ToString());
                log?.Invoke($"Interface: {i.Identifier.ToString()}");
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
                        var type = ((member as FieldDeclarationSyntax).Declaration as VariableDeclarationSyntax).Type;
                        if (type is IdentifierNameSyntax)
                        {
                            coll.SetAssociation(c.Identifier.ToString(), (type as IdentifierNameSyntax).Identifier.Text);
                            log?.Invoke($"{c.Identifier.ToString()} associates to {(type as IdentifierNameSyntax).Identifier.Text}");
                        }
                        else if (type is GenericNameSyntax)
                        {
                            coll.SetAssociation(c.Identifier.ToString(), (type as GenericNameSyntax).Identifier.Text);
                            foreach (var arg in (type as GenericNameSyntax).TypeArgumentList.Arguments)
                            {
                                coll.SetAssociation(c.Identifier.ToString(), arg.ToString());
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
