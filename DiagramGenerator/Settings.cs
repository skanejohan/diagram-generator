using System;
using System.IO;

namespace DiagramGenerator
{
    public class Settings
    {
        public bool IncludePrivateReferences { get; }
        public bool IncludeAssociations { get; }
        public bool IncludeInheritance { get; }

        public Settings(string settingsFile)
        {
            if (File.Exists(settingsFile))
            {
                var f = new ConfigFile(settingsFile);
                IncludePrivateReferences = f.ReadBool("include_private_references");
                IncludeAssociations = f.ReadBool("include_associations");
                IncludeInheritance = f.ReadBool("include_inheritance");
            }
        }
    }

}
