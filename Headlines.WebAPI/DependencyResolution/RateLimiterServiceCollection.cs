using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

namespace Headlines.WebAPI.DependencyResolution
{
    public static class RateLimiterServiceCollection
    {
        public static IServiceCollection AddRateLimiterDependencyGroup(this IServiceCollection services)
        {
            services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.RejectionStatusCode = 429;

                limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (remoteIpAddress is null)
                        return RateLimitPartition.GetNoLimiter($"TEST_LIMITER");

                    if (IPAddress.IsLoopback(remoteIpAddress!))
                        return RateLimitPartition.GetNoLimiter($"LOOPBACK_{IPAddress.Loopback}");

                    return context.Request.Method switch {
                        "POST" => RateLimitPartition.GetTokenBucketLimiter($"POST_{remoteIpAddress}", _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 3,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 1,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                            TokensPerPeriod = 2,
                            AutoReplenishment = true
                        }),
                        "GET" => RateLimitPartition.GetTokenBucketLimiter($"GET_{remoteIpAddress}", _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 20,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 1,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(3),
                            TokensPerPeriod = 10,
                            AutoReplenishment = true
                        }),
                        _ => RateLimitPartition.GetConcurrencyLimiter($"DEFAULT_{remoteIpAddress}", _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = 1,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0,
                        })
                    };
                });
            });

            return services;
        }
    }
}