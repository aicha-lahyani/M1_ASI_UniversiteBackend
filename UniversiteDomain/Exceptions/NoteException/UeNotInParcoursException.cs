namespace UniversiteDomain.Exceptions.NoteException;

public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException() : base() { }
    public UeNotInParcoursException(string message) : base(message) { }
    public UeNotInParcoursException(string message, Exception inner) : base(message, inner) { }
}