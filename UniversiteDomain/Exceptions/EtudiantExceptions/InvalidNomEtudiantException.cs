namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class InvalidNomEtudiantException:Exception
{
    public InvalidNomEtudiantException(string message) : base(message) { }
}