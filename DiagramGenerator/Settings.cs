using System.IO;

namespace DiagramGenerator
{
    public class Settings
    {
        public bool IncludePublicAssociations { get; set; }
        public bool IncludeProtectedAssociations { get; set; }
        public bool IncludeInternalAssociations { get; set; }
        public bool IncludePrivateAssociations { get; set; }
        public bool IncludeInheritance { get; set; }
        public string StartClass { get; set; }

        public Settings(string settingsFile = "")
        {
            IncludePublicAssociations = true;
            IncludeProtectedAssociations = false;
            IncludeInternalAssociations = false;
            IncludePrivateAssociations = false;
            IncludeInheritance = true;
            StartClass = "";
            if (File.Exists(settingsFile))
            {
                var f = new ConfigFile(settingsFile);
                IncludePublicAssociations = f.ReadBool("include_public_associations", IncludePublicAssociations);
                IncludeProtectedAssociations = f.ReadBool("include_protected_associations", IncludeProtectedAssociations);
                IncludeInternalAssociations = f.ReadBool("include_internal_associations", IncludeInternalAssociations);
                IncludePrivateAssociations = f.ReadBool("include_private_associations", IncludePrivateAssociations);
                IncludeInheritance = f.ReadBool("include_inheritance", IncludeInheritance);
                StartClass = f.ReadString("start_class", StartClass);
            }
        }
    }

}
