namespace UniversiteDomain.Exceptions.UeExceptions
{
    public class UeNotFoundException : Exception
    {
        public UeNotFoundException(string message) : base(message) { }
    }

    public class DuplicateUeDansParcoursException : Exception
    {
        public DuplicateUeDansParcoursException(string message) : base(message) { }
    }

    public class InvalidNumeroUeException : Exception
    {
        public InvalidNumeroUeException(string message) : base(message) { }
    }

    public class InvalidIntituleUeException : Exception
    {
        public InvalidIntituleUeException(string message) : base(message) { }
    }
}