namespace UniversiteDomain.Exceptions.NoteException;

public class InvalidNoteException : Exception
{
    public InvalidNoteException() : base() { }
    public InvalidNoteException(string message) : base(message) { }
    public InvalidNoteException(string message, Exception inner) : base(message, inner) { }
}