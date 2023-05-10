namespace RushHour.Domain.Exceptions
{
    public class IncorrectAccountIdException : Exception
    {
        public IncorrectAccountIdException() { }

        public IncorrectAccountIdException(string id)
            : base($"The id: {id} is in incorrect format") { }

        public IncorrectAccountIdException(string id, Exception inner)
            : base($"The id: {id} is in incorrect format", inner) { }
    }
}
