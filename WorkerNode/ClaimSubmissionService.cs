namespace WorkerNode
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;


    public class ClaimSubmissionService :
        BackgroundService
    {
        readonly IBusControl _bus;
        readonly ILogger<ClaimSubmissionService> _logger;
        readonly IOptions<StartupOptions> _options;

        public ClaimSubmissionService(ILogger<ClaimSubmissionService> logger, IBusControl bus, IOptions<StartupOptions> options)
        {
            _logger = logger;
            _bus = bus;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.WaitForHealthStatus(BusHealthStatus.Healthy, stoppingToken);

            IRequestClient<SubmitClaim> client = _bus.CreateRequestClient<SubmitClaim>();

            if (_options.Value.Count.HasValue)
            {
                var count = _options.Value.Count.Value;

                await Task.Delay(200, stoppingToken);

                _logger.LogError("Sending: {0}", count);

                var timer = Stopwatch.StartNew();

                for (var i = 0; i < count / 40; i++)
                {
                    Response<ClaimSubmitted>[] responses = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ =>
                        client.GetResponse<ClaimSubmitted>(new SubmitClaim
                        {
                            ClaimId = NewId.NextGuid(),
                        }, stoppingToken))).ConfigureAwait(false);
                }

                timer.Stop();

                _logger.LogError("Send count: {0}", count);
                _logger.LogError("Total send duration: {0:g}", timer.Elapsed);
                _logger.LogError("Request rate: {0:F2} (req/s)", count * 1000 / timer.Elapsed.TotalMilliseconds);
            }
            else if (!string.IsNullOrWhiteSpace(_options.Value.Servers))
            {
                for (var i = 0; i < 10; i++)
                {
                    await Task.Delay(200, stoppingToken);

                    Response<ClaimSubmitted> response = await client.GetResponse<ClaimSubmitted>(new SubmitClaim
                    {
                        ClaimId = NewId.NextGuid(),
                    }, stoppingToken);

                    _logger.LogInformation("Claim Submitted: {ClaimId} - {SourceAddress}", response.Message.ClaimId, response.SourceAddress);
                }
            }
        }
    }
}