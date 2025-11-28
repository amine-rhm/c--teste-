namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class InvalidNomParcoursException : Exception
{
    public InvalidNomParcoursException() { }

    public InvalidNomParcoursException(string message)
        : base(message) { }

    public InvalidNomParcoursException(string message, Exception innerException)
        : base(message, innerException) { }
}