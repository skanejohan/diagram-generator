# diagram-generator

This application generates a text file representing a class hierarchy. The text file is on the PlantUML format and can be viewed with a suitable viewer, such as the [PlantUML viewer plugin](https://chrome.google.com/webstore/detail/plantuml-viewer/legbfeljfbjgfifnkmpoajgpgejojooj) for Chrome.

## Generating diagrams

To perform analysis, run the application, specifying the base directory of all your source code. The application will look for C# files in this directory and all its subdirectories, and build an internal representation of the structure of your code.

This internal structure can be presented in a number of different ways - you may want to create inheritance hierarchies of all the code or of parts thereof, you may want to present associations between the different classes or you may want a combination of the two. You may also want to present the whole structure or (more likely) create different diagrams representing different parts of your code. How the system presents your code structure is determined by **configuration files**.

The application looks for configuration files the same way that it looks for C# files, which means that configuration files may be located anywhere in your directory tree. I find it useful to create a special directory for the solution (or one per project for a larger code base) and place all my configuration files in there. Configuration files have extension ".dg.cfg". Each configuration will cause a PlantUML file with the same base name to be created next to it. Assuming for example that we have two configuration files - **inheritance.dg.cfg** and **associations.dg.cfg** - in a directory, the application will generate the files **inheritance.plantuml** and **associations.plantuml** in the same directory.

## Structure of a configuration file

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=true
start_class=SimpleClass1
```

The configuration file above contains all settings available. 
 - **include_public_associations** indicates that a public association from one class to another should be included. 
 - **include_protected_associations** indicates that a protected association from one class to another should be included. 
 - **include_internal_associations** indicates that an internal association from one class to another should be included. 
 - **include_private_associations** indicates that a private association from one class to another should be included. 
 - **include_inheritance** indicates that inheritance information should be included. 
 - **start_class** indicates a class that is the "start" of the diagram. Classes and interfaces from which this class derives are included, as well as associations starting in this class. If this entry is not found in the configuration file, all classes and interfaces found are included in the diagram.

## Examples

The following examples use the Sample.cs file containing a set of classes and interfaces, with inheritance and association.

### All

all.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=true
```

all.plantuml:

![all.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501061/7e515a5a-b038-11e6-9c6c-50bdf660d257.png)

### SimpleClass1_NoInheritance

SimpleClass1_NoInheritance.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=false
start_class=SimpleClass1
```

SimpleClass1_NoInheritance.plantuml:

![SimpleClass1_NoInheritance.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501548/a08aa084-b03a-11e6-8843-a3e4086324ce.png)

### SimpleClass1

SimpleClass1.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=true
start_class=SimpleClass1
```

SimpleClass1.plantuml:

![SimpleClass1.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501426/2de9546c-b03a-11e6-9054-3f80ecf418c9.png)

### SimpleClass1_PublicProtectedInternal

SimpleClass1_PublicProtectedInternal.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=false
start_class=SimpleClass1
```

SimpleClass1_PublicProtectedInternal.plantuml:

![SimpleClass1_PublicProtectedInternal.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501890/023f8c8a-b03c-11e6-8184-60231486a8bb.png)

### SimpleClass1_PublicProtected

SimpleClass1_PublicProtected.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=false
start_class=SimpleClass1
```

SimpleClass1_PublicProtected.plantuml:

![SimpleClass1_PublicProtected.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501891/023fec34-b03c-11e6-9753-e07cf0c89767.png)

### SimpleClass1_Public

SimpleClass1_Public.dg.cfg:

```
include_public_associations=true
include_protected_associations=true
include_internal_associations=true
include_private_associations=true
include_inheritance=false
start_class=SimpleClass1
```

SimpleClass1_Public.plantuml:

![SimpleClass1_Public.plantuml](https://cloud.githubusercontent.com/assets/4025939/20501892/0246420a-b03c-11e6-9fe5-f1a4a51b3777.png)

