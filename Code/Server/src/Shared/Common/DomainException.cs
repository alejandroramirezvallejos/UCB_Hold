namespace Shared.Common
{
    public class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }
}
