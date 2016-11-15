using System.Collections.Generic;
using System.Linq;

namespace DiagramGenerator
{
    public class CSharpInterface
    {
        public string Name { get; set; }
    }

    public class CSharpClass : CSharpInterface
    {
        public CSharpClass Derives { get; internal set; }
        public IEnumerable<CSharpInterface> Implements => implements;
        public IEnumerable<CSharpInterface> InterfaceAssociations => interfaceAssociations;
        public IEnumerable<CSharpClass> ClassAssociations => classAssociations;

        public CSharpClass()
        {
            implements = new List<CSharpInterface>();
            interfaceAssociations = new List<CSharpInterface>();
            classAssociations = new List<CSharpClass>();
        }

        private List<CSharpInterface> implements;
        private List<CSharpInterface> interfaceAssociations;
        private List<CSharpClass> classAssociations;

        internal void ExtendsInterface(CSharpInterface i)
        {
            implements.Add(i);
        }

        internal void AddInterfaceAssociation(CSharpInterface i)
        {
            if (!interfaceAssociations.Contains(i))
            {
                interfaceAssociations.Add(i);
            }
        }

        internal void AddClassAssociation(CSharpClass c)
        {
            if (!classAssociations.Contains(c))
            {
                classAssociations.Add(c);
            }
        }
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

        public void AddClass(string name)
        {
            classes.Add(new CSharpClass { Name = name });
        }

        public void AddInterface(string name)
        {
            interfaces.Add(new CSharpInterface { Name = name });
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

        public void SetAssociation(string className, string associatesTo)
        {
            var thisClass = classes.FirstOrDefault(c => c.Name == className);
            var associatedClass = classes.FirstOrDefault(c => c.Name == associatesTo);
            var associatedInterface = interfaces.FirstOrDefault(i => i.Name == associatesTo);

            if (thisClass != null)
            {
                if (associatedClass != null)
                {
                    thisClass.AddClassAssociation(associatedClass);
                }
                else if (associatedInterface != null)
                {
                    thisClass.AddInterfaceAssociation(associatedInterface);
                }
            }
        }
    }
}
