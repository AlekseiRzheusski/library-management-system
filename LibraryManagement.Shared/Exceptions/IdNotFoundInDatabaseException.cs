namespace LibraryManagement.Shared.Exceptions;

public class IdNotFoundInDatabaseException : Exception
{
    public IdNotFoundInDatabaseException(string message)
        : base(message) { }
}
