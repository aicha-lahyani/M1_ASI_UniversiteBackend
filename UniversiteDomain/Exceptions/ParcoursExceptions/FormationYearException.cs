namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class FormationYearException : Exception
{
    public FormationYearException() : base() { }
    public FormationYearException(string message) : base(message) { }
    public FormationYearException(string message, Exception inner) : base(message, inner) { }
}