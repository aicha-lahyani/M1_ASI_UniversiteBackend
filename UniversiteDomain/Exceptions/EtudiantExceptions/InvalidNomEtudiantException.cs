namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class InvalidNomEtudiantException : Exception
{
    public InvalidNomEtudiantException() : base() { }
    public InvalidNomEtudiantException(string message) : base(message) { }
    public InvalidNomEtudiantException(string message, Exception inner) : base(message, inner) { }
}