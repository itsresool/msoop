using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Msoop.Web.Reddit.Exceptions;

namespace Msoop.Web.Reddit
{
    public class RateLimitHandler : DelegatingHandler
    {
        private const string RemainingHeaderName = "X-Ratelimit-Remaining";
        private const string NextPeriodHeaderName = "X-Ratelimit-Reset";
        private DateTimeOffset _nextPeriodUtcAt = DateTimeOffset.UtcNow.AddMinutes(minutes: 1);
        private int _requestsRemaining = 60;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_requestsRemaining == 0)
            {
                throw new RateLimitedException(_nextPeriodUtcAt);
            }

            var response = await base.SendAsync(request, cancellationToken);
            UpdateRateLimitValues(response);

            return response;
        }

        private void UpdateRateLimitValues(HttpResponseMessage response)
        {
            if (response.Headers.Contains(RemainingHeaderName))
            {
                var value = response.Headers.GetValues(RemainingHeaderName).First();
                _requestsRemaining = int.Parse(value);
            }

            if (response.Headers.Contains(NextPeriodHeaderName))
            {
                var value = response.Headers.GetValues(NextPeriodHeaderName).First();
                var secondsToNextPeriod = int.Parse(value);
                _nextPeriodUtcAt = DateTimeOffset.UtcNow.AddSeconds(secondsToNextPeriod);
            }
        }
    }
}
