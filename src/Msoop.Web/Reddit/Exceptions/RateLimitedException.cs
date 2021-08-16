using System;

namespace Msoop.Web.Reddit.Exceptions
{
    public class RateLimitedException : Exception
    {
        public RateLimitedException(DateTimeOffset time) : base("You are rate limited")
        {
            AttemptAgainAtUtc = time;
        }

        public RateLimitedException(string message) : base(message)
        {
        }

        public RateLimitedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DateTimeOffset AttemptAgainAtUtc { get; init; }
    }
}
