namespace UniversiteDomain.Exceptions.NoteException;

public class NoteImportException : Exception
{
    public NoteImportException() : base() { }
    public NoteImportException(string message) : base(message) { }
    public NoteImportException(string message, Exception inner) : base(message, inner) { }
}