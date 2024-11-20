namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateInscripionException : Exception
{
    public DuplicateInscripionException() : base() { }
    public DuplicateInscripionException(string message) : base(message) { }
    public DuplicateInscripionException(string message, Exception inner) : base(message, inner) { }
}