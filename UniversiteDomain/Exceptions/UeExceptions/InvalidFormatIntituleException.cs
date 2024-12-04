namespace UniversiteDomain.Exceptions.UeExceptions;

public class InvalidFormatIntituleException : Exception
{
    public InvalidFormatIntituleException() : base() { }
    public InvalidFormatIntituleException(string message) : base(message) { }
    public InvalidFormatIntituleException(string message, Exception inner) : base(message, inner) { }
}