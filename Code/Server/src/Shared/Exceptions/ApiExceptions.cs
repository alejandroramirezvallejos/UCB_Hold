namespace IMT_Reservas.Server.Shared.Common
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}

