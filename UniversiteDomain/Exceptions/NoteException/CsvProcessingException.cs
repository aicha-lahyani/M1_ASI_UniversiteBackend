namespace UniversiteDomain.Exceptions.NoteException;

public class CsvProcessingException : Exception
{
    public CsvProcessingException() : base() { }
    public CsvProcessingException(string message) : base(message) { }
    public CsvProcessingException(string message, Exception inner) : base(message, inner) { }
}