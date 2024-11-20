namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateParcoursException : Exception
{
    public DuplicateParcoursException() : base() { }
    public DuplicateParcoursException(string message) : base(message) { }
    public DuplicateParcoursException(string message, Exception inner) : base(message, inner) { }
}