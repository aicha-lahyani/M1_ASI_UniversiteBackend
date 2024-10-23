namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException() : base() { }
    public DuplicateEmailException(string message) : base(message) { }
    public DuplicateEmailException(string message, Exception inner) : base(message, inner) { }
}