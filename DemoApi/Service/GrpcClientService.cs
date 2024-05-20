namespace DemoApi.Service;

using DemoMicroService;
using Grpc.Core;
using Grpc.Net.Client;
using Polly;
using Polly.Retry;
using TokenRequest = DemoApi.Models.Request.TokenRequest;

public class GrpcClientService : IGrpcClientService
{
    private readonly GrpcChannel serverChannel;
    private readonly ResiliencePipeline retryPipeline;

    public GrpcClientService(ILoggerFactory loggerFactory)
    {
        var serverAddress = new Uri("http://localhost:5122");

        this.serverChannel = GrpcChannel.ForAddress(
            serverAddress, new GrpcChannelOptions()
            {
                Credentials = ChannelCredentials.Insecure,
                LoggerFactory = loggerFactory,
            });

        var retryStrategyOptions = new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true, // Adds a random factor to the delay
            MaxRetryAttempts = 10,
            Delay = TimeSpan.FromSeconds(1),
        };

        this.retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(retryStrategyOptions)
            .Build();
    }

    public async Task<string> GetAuthTokenAsync(TokenRequest request)
    {
        var tokenServiceClient = new TokenService.TokenServiceClient(this.serverChannel);

        var tokenRequest = new DemoMicroService.TokenRequest
        {
            Username = request.Username,
            Password = request.Password,
        };

        var tokenReply = await this.retryPipeline.ExecuteAsync(
            async (token) =>
            {
                var tokenReply = await tokenServiceClient.TokenAsync(tokenRequest, cancellationToken: token);

                return tokenReply;
            });

        return tokenReply.Token;
    }
}
