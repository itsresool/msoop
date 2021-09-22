using Microsoft.Extensions.DependencyInjection;
using Msoop.Web.Reddit;
using Msoop.Web.Reddit.Handlers;

namespace Msoop.Web.Extensions
{
    public static class StartupExtensions
    {
        public static IHttpClientBuilder AddRedditService(this IServiceCollection services)
        {
            services.AddTransient<AuthorizationHandler>();
            services.AddTransient<RateLimitHandler>();

            return services.AddHttpClient<IRedditService, RedditService>()
                .AddHttpMessageHandler<AuthorizationHandler>()
                .AddHttpMessageHandler<RateLimitHandler>();
        }
    }
}
