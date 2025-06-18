using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using ReqResApiIntegration.Config;
using ReqResApiIntegration.Services;
using System.Net;
using System.Net.Http;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ReqResOptions>(context.Configuration.GetSection("ReqResApi"));
        services.AddMemoryCache();


        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
        .HandleResult<HttpResponseMessage>(r =>
            r.StatusCode == HttpStatusCode.RequestTimeout ||
            r.StatusCode == HttpStatusCode.InternalServerError ||
            r.StatusCode == HttpStatusCode.BadGateway ||
            r.StatusCode == HttpStatusCode.ServiceUnavailable ||
            r.StatusCode == HttpStatusCode.GatewayTimeout)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        services.AddHttpClient<IExternalUserService, ExternalUserService>()
            .ConfigureHttpClient((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var settings = config.GetSection("ReqResApi").Get<ReqResOptions>();
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
            })
            .AddHttpMessageHandler(() => new PolicyHttpMessageHandler(retryPolicy));

        services.AddTransient<ExternalUserService>();
    })
    .Build();

var service = host.Services.GetRequiredService<IExternalUserService>();
var users = await service.GetAllUsersAsync();
Console.WriteLine($"Fetched {users.Count()} users from ReqRes API");


class PolicyHttpMessageHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public PolicyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy)
    {
        _policy = policy;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }
}