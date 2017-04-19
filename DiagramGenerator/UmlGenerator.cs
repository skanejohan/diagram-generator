using System.Collections.Generic;

namespace DiagramGenerator
{
    public class UmlGenerator
    {
        public List<string> GeneratePlantUml(CSharpObjectCollection coll)
        {
            var uml = new List<string>();

            uml.Add("@startuml");
            foreach (var i in coll.Interfaces)
            {
                uml.Add($"interface {i.Name}");
            }

            foreach (var cls in coll.Classes)
            {
                uml.Add($"class {cls.Name}");
                if (cls.Derives != null)
                {
                    uml.Add($"{cls.Derives.Name} <|-- {cls.Name}");
                }
                foreach (var i in cls.Implements)
                {
                    uml.Add($"{i.Name} <|-- {cls.Name}");
                }
                foreach (var ia in cls.InterfaceAssociations)
                {
                    uml.Add($"{ia.Interface.Name} <-- {cls.Name}");
                }
                foreach (var ca in cls.ClassAssociations)
                {
                    uml.Add($"{ca.Class.Name} <-- {cls.Name}");
                }
            }
            uml.Add("@enduml");

            return uml;
        }

    }
}
