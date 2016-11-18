using System.IO;

namespace DiagramGenerator
{
    public class Settings
    {
        public bool IncludePublicAssociations { get; }
        public bool IncludeProtectedAssociations { get; }
        public bool IncludeInternalAssociations { get; }
        public bool IncludePrivateAssociations { get; }
        public bool IncludeInheritance { get; }
        public string StartClass { get; }

        public Settings(string settingsFile)
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
