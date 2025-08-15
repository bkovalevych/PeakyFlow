namespace PeakyFlow.Abstractions.Common.Exceptions
{
    public class AppPreconditionFailedException : Exception
    {
        public bool CanBeRetried { get; set; }

        public AppPreconditionFailedException(bool canBeRetried, string? message, Exception inner) : base(message, inner) 
        {
            CanBeRetried = canBeRetried;
        }
    }
}
