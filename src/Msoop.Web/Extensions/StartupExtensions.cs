using Microsoft.Extensions.DependencyInjection;
using Msoop.Web.Reddit;

namespace Msoop.Web.Extensions
{
    public static class StartupExtensions
    {
        public static IHttpClientBuilder AddRedditService(this IServiceCollection services)
        {
            services.AddTransient<AuthorizationHandler>();
            services.AddTransient<RateLimitHandler>();

            return services.AddHttpClient<RedditService>()
                .AddHttpMessageHandler<AuthorizationHandler>()
                .AddHttpMessageHandler<RateLimitHandler>();
        }
    }
}
