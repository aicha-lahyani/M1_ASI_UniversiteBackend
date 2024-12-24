namespace UniversiteDomain.Exceptions.NoteException;

public class DuplicateNoteForUeException : Exception
{
    public DuplicateNoteForUeException() : base() { }
    public DuplicateNoteForUeException(string message) : base(message) { }
    public DuplicateNoteForUeException(string message, Exception inner) : base(message, inner) { }
}