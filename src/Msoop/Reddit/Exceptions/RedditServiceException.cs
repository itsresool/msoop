using System;

namespace Msoop.Reddit.Exceptions
{
    public class RedditServiceException : Exception
    {
        public RedditServiceException()
        {
        }

        public RedditServiceException(string message) : base(message)
        {
        }

        public RedditServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
