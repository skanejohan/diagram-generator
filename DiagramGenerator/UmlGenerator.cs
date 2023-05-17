using System.Collections.Generic;

namespace DiagramGenerator
{
    public static class UmlGenerator
    {
        public static List<string> GeneratePlantUml(CSharpObjectCollection coll)
        {
            List<string> uml = new()
            {
                "@startuml"
            };

            foreach (CSharpInterface i in coll.Interfaces)
            {
                uml.Add($"interface {i.Name}");
            }

            foreach (CSharpClass cls in coll.Classes)
            {
                uml.Add($"class {cls.Name}");
                if (cls.Derives != null)
                {
                    uml.Add($"{cls.Derives.Name} <|-- {cls.Name}");
                }
                foreach (CSharpInterface i in cls.Implements)
                {
                    uml.Add($"{i.Name} <|-- {cls.Name}");
                }
                foreach (InterfaceAssociation ia in cls.InterfaceAssociations)
                {
                    uml.Add($"{ia.Interface.Name} <-- {cls.Name}");
                }
                foreach (ClassAssociation ca in cls.ClassAssociations)
                {
                    uml.Add($"{ca.Class.Name} <-- {cls.Name}");
                }
            }
            uml.Add("@enduml");

            return uml;
        }
    }
}