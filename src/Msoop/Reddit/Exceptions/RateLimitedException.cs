using System;

namespace Msoop.Reddit.Exceptions
{
    public class RateLimitedException : Exception
    {
        public DateTimeOffset AttemptAgainAtUtc { get; init; }

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
    }
}
