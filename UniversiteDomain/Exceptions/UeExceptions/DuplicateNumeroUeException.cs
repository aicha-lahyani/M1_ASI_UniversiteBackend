namespace UniversiteDomain.Exceptions.UeExceptions;

public class DuplicateNumeroUeException : Exception
{
    public DuplicateNumeroUeException() : base() { }
    public DuplicateNumeroUeException(string message) : base(message) { }
    public DuplicateNumeroUeException(string message, Exception inner) : base(message, inner) { }
}