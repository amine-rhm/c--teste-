namespace UniversiteDomain.Exceptions;

public class DuplicateInscriptionException : Exception
{
    public DuplicateInscriptionException(string message) : base(message) { }
}