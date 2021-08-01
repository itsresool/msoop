using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Msoop.Reddit
{
    public class RateLimitHandler : DelegatingHandler
    {
        private const string RemainingHeaderName = "X-Ratelimit-Remaining";
        private const string NextPeriodHeaderName = "X-Ratelimit-Reset";
        private int _requestsRemaining = 60;
        private int _secondsToNextPeriod = 60;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_requestsRemaining == 0)
            {
                throw new Exception($"Try again in {_secondsToNextPeriod} seconds");
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
                _secondsToNextPeriod = int.Parse(value);
            }
        }
    }
}
