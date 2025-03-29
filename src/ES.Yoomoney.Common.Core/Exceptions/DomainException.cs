namespace ES.Yoomoney.Common.Core.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DomainException(string message) : base(message)
    {
    }
}