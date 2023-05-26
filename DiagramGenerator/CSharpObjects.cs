using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace DiagramGenerator
{
    public enum CSharpVisibility
    {
        Private,
        Protected,
        Internal,
        Public
    }

    [Serializable()]
    public class CSharpInterface
    {
        public string Name { get; set; }

        public void Clone(CSharpObjectCollection coll)
        {
            coll.AddInterface(Name);
        }
    }

    [Serializable()]
    public class InterfaceAssociation
    {
        public CSharpInterface Interface { get; set; }
        public CSharpVisibility Visibility { get; set; }
    }

    [Serializable()]
    public class CSharpClass : CSharpInterface
    {
        private readonly List<CSharpInterface> implements;
        private readonly List<InterfaceAssociation> interfaceAssociations;
        private readonly List<ClassAssociation> classAssociations;

        public CSharpClass()
        {
            implements = new List<CSharpInterface>();
            interfaceAssociations = new List<InterfaceAssociation>();
            classAssociations = new List<ClassAssociation>();
        }

        public CSharpClass Derives { get; internal set; }
        public IEnumerable<CSharpInterface> Implements => implements;
        public IEnumerable<InterfaceAssociation> InterfaceAssociations => interfaceAssociations;
        public IEnumerable<ClassAssociation> ClassAssociations => classAssociations;

        public void Clone(CSharpObjectCollection coll, Settings settings, int depth)
        {
            coll.AddClass(Name);

            if (depth > 0)
            {
                foreach (ClassAssociation c in ClassAssociations)
                {
                    if (
                        (c.Visibility == CSharpVisibility.Public && settings.IncludePublicAssociations) ||
                        (c.Visibility == CSharpVisibility.Protected && settings.IncludeProtectedAssociations) ||
                        (c.Visibility == CSharpVisibility.Internal && settings.IncludeInternalAssociations) ||
                        (c.Visibility == CSharpVisibility.Private && settings.IncludePrivateAssociations)
                       )
                    {
                        if (!coll.ClassExists(c.Class.Name))
                        {
                            c.Class.Clone(coll, settings, depth - 1);
                        }
                        coll.SetAssociation(Name, c.Class.Name, c.Visibility);
                    }
                }

                foreach (InterfaceAssociation i in InterfaceAssociations)
                {
                    if (
                        (i.Visibility == CSharpVisibility.Public && settings.IncludePublicAssociations) ||
                        (i.Visibility == CSharpVisibility.Protected && settings.IncludeProtectedAssociations) ||
                        (i.Visibility == CSharpVisibility.Internal && settings.IncludeInternalAssociations) ||
                        (i.Visibility == CSharpVisibility.Private && settings.IncludePrivateAssociations)
                       )
                    {
                        if (!coll.InterfaceExists(i.Interface.Name))
                        {
                            i.Interface.Clone(coll);
                        }
                        coll.SetAssociation(Name, i.Interface.Name, i.Visibility);
                    }
                }

                if (settings.IncludeInheritance)
                {
                    foreach (CSharpInterface i in Implements)
                    {
                        if (!coll.InterfaceExists(i.Name))
                        {
                            coll.AddInterface(i.Name);
                        }
                        coll.SetExtends(Name, i.Name);
                    }
                    if (Derives != null)
                    {
                        if (!coll.ClassExists(Derives.Name))
                        {
                            Derives.Clone(coll, settings, depth - 1);
                        }
                        coll.SetExtends(Name, Derives.Name);
                    }
                }
            }
        }

        internal void ExtendsInterface(CSharpInterface i)
        {
            implements.Add(i);
        }

        internal void AddInterfaceAssociation(CSharpInterface i, CSharpVisibility visibility)
        {
            InterfaceAssociation ia = interfaceAssociations.FirstOrDefault(a => a.Interface == i);
            if (ia == null)
            {
                interfaceAssociations.Add(new InterfaceAssociation
                {
                    Interface = i,
                    Visibility = visibility
                });
            }
            else if (ia.Visibility < visibility)
            {
                ia.Visibility = visibility;
            }
        }

        internal void AddClassAssociation(CSharpClass c, CSharpVisibility visibility)
        {
            ClassAssociation ca = classAssociations.FirstOrDefault(a => a.Class == c);
            if (ca == null)
            {
                classAssociations.Add(new ClassAssociation
                {
                    Class = c,
                    Visibility = visibility
                });
            }
            else if (ca.Visibility < visibility)
            {
                ca.Visibility = visibility;
            }
        }
    }

    [Serializable()]
    public class ClassAssociation
    {
        public CSharpClass Class { get; set; }
        public CSharpVisibility Visibility { get; set; }
    }

    [Serializable()]
    public class CSharpObjectCollection
    {
        private readonly List<CSharpClass> classes;
        private readonly List<CSharpInterface> interfaces;

        public CSharpObjectCollection()
        {
            classes = new List<CSharpClass>();
            interfaces = new List<CSharpInterface>();
        }

        public IEnumerable<CSharpClass> Classes => classes;
        public IEnumerable<CSharpInterface> Interfaces => interfaces;

        public static CSharpObjectCollection Deserialize(string fileName)
        {
            //var coll = new CSharpObjectCollection();
            Stream s = File.OpenRead(fileName);
            BinaryFormatter serializer = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            object coll = serializer.Deserialize(s);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            s.Close();
            return coll as CSharpObjectCollection;
        }

        public bool ClassExists(string name)
        {
            return classes.FirstOrDefault(c => c.Name == name) != null;
        }

        public void AddClass(string name)
        {
            if (!ClassExists(name))
            {
                classes.Add(new CSharpClass { Name = name });
            }
        }

        public bool InterfaceExists(string name)
        {
            return interfaces.FirstOrDefault(c => c.Name == name) != null;
        }

        public void AddInterface(string name)
        {
            if (!InterfaceExists(name))
            {
                interfaces.Add(new CSharpInterface { Name = name });
            }
        }

        public void SetExtends(string className, string extends)
        {
            CSharpClass thisClass = classes.FirstOrDefault(c => c.Name == className);
            CSharpClass extendedClass = classes.FirstOrDefault(c => c.Name == extends);
            CSharpInterface extendedInterface = interfaces.FirstOrDefault(i => i.Name == extends);

            if (thisClass != null)
            {
                if (extendedClass != null)
                {
                    thisClass.Derives = extendedClass;
                }
                else if (extendedInterface != null)
                {
                    thisClass.ExtendsInterface(extendedInterface);
                }
            }
        }

        public void SetAssociation(string className, string associatesTo, CSharpVisibility visibility)
        {
            CSharpClass thisClass = classes.FirstOrDefault(c => c.Name == className);
            CSharpClass associatedClass = classes.FirstOrDefault(c => c.Name == associatesTo);
            CSharpInterface associatedInterface = interfaces.FirstOrDefault(i => i.Name == associatesTo);

            if (thisClass != null)
            {
                if (associatedClass != null)
                {
                    thisClass.AddClassAssociation(associatedClass, visibility);
                }
                else if (associatedInterface != null)
                {
                    thisClass.AddInterfaceAssociation(associatedInterface, visibility);
                }
            }
        }

        public CSharpObjectCollection Clone(string startClass, Settings settings, int depth)
        {
            CSharpObjectCollection coll = new();
            classes.FirstOrDefault(c => c.Name == startClass)?.Clone(coll, settings, depth);
            return coll;
        }

        public void Serialize(string fileName)
        {
            Stream s = File.Create(fileName);
            BinaryFormatter serializer = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            serializer.Serialize(s, this);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            s.Close();
        }
    }
}