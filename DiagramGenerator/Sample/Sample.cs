namespace DiagramGenerator.Sample
{
    public interface Interface1
    {
    }

    public interface Interface2
    {
    }


    public class SimpleClass1
    {
        public SimpleClass2 SimpleClass2;
        public SimpleClass3 SimpleClass3;
    }

    public class SimpleClass2
    {
        public DerivedClass DerivedClass;
    }

    public class DerivedClass : ImplementingClass, Interface2
    {
        public ClassWithAllAssociations ClassWithAllAssociations;
    }

    public class ClassWithAllAssociations
    {
        public PublicClass PublicClass;
        protected ProtectedClass ProtectedClass;
        internal InternalClass InternalClass;
        private PrivateClass PrivateClass;
    }

    public class ImplementingClass : Interface1
    {
    }

    public class DerivedUnusedClass1 : DerivedClass
    {
    }

    public class DerivedUnusedClass2 : DerivedUnusedClass1
    {
    }

    public class SimpleClass2 SimpleClass2
    {
    }

    public class PublicClass
    {
    }

    public class ProtectedClass
    {
    }

    public class InternalClass
    {
    }

    public class PrivateClass
    {
    }
}
