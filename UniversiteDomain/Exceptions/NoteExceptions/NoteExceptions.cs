namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException(string message) : base(message) { }
}

public class DuplicateNoteException : Exception
{
    public DuplicateNoteException(string message) : base(message) { }
}

public class InvalidNoteValeurException : Exception
{
    public InvalidNoteValeurException(string message) : base(message) { }
}

public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException(string message) : base(message) { }
}