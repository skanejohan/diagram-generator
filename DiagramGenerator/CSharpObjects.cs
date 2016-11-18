using System.Collections.Generic;
using System.Linq;

namespace DiagramGenerator
{
    public enum CSharpVisibility
    {
        Private,
        Protected,
        Internal,
        Public
    }

    public class CSharpInterface
    {
        public string Name { get; set; }
    }

    public class InterfaceAssociation
    {
        public CSharpInterface Interface { get; set; }
        public CSharpVisibility Visibility { get; set; }
    }

    public class CSharpClass : CSharpInterface
    {
        public CSharpClass Derives { get; internal set; }
        public IEnumerable<CSharpInterface> Implements => implements;
        public IEnumerable<InterfaceAssociation> InterfaceAssociations => interfaceAssociations;
        public IEnumerable<ClassAssociation> ClassAssociations => classAssociations;

        public CSharpClass()
        {
            implements = new List<CSharpInterface>();
            interfaceAssociations = new List<InterfaceAssociation>();
            classAssociations = new List<ClassAssociation>();
        }

        private readonly List<CSharpInterface> implements;
        private readonly List<InterfaceAssociation> interfaceAssociations;
        private readonly List<ClassAssociation> classAssociations;

        internal void ExtendsInterface(CSharpInterface i)
        {
            implements.Add(i);
        }

        internal void AddInterfaceAssociation(CSharpInterface i, CSharpVisibility visibility)
        {
            var ia = interfaceAssociations.FirstOrDefault(a => a.Interface == i);
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
            var ca = classAssociations.FirstOrDefault(a => a.Class == c);
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

        public void Clone(CSharpObjectCollection coll, Settings settings)
        {
            coll.AddClass(Name);

            foreach (var c in ClassAssociations)
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
                        c.Class.Clone(coll, settings);
                    }
                    coll.SetAssociation(Name, c.Class.Name, c.Visibility);
                }
            }

            if (settings.IncludeInheritance)
            {
                foreach (var i in Implements)
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
                        Derives.Clone(coll, settings);
                    }
                    coll.SetExtends(Name, Derives.Name);
                }
            }
        }
    }

    public class ClassAssociation
    {
        public CSharpClass Class { get; set; }
        public CSharpVisibility Visibility { get; set; }
    }

    public class CSharpObjectCollection
    {
        public IEnumerable<CSharpClass> Classes => classes;
        public IEnumerable<CSharpInterface> Interfaces => interfaces;

        private List<CSharpClass> classes;
        private List<CSharpInterface> interfaces;

        public CSharpObjectCollection()
        {
            classes = new List<CSharpClass>();
            interfaces = new List<CSharpInterface>();
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
            var thisClass = classes.FirstOrDefault(c => c.Name == className);
            var extendedClass = classes.FirstOrDefault(c => c.Name == extends);
            var extendedInterface = interfaces.FirstOrDefault(i => i.Name == extends);

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
            var thisClass = classes.FirstOrDefault(c => c.Name == className);
            var associatedClass = classes.FirstOrDefault(c => c.Name == associatesTo);
            var associatedInterface = interfaces.FirstOrDefault(i => i.Name == associatesTo);

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

        public CSharpObjectCollection Clone(string startClass, Settings settings)
        {
            var coll = new CSharpObjectCollection();
            classes.FirstOrDefault(c => c.Name == startClass)?.Clone(coll, settings);
            return coll;
        }
    }
}
