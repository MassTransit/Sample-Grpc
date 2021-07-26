namespace WorkerNode
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.MultiBus;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Serilog;


    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    if (host.HostingEnvironment.IsProduction())
                        log.MinimumLevel.Information();
                    else
                        log.MinimumLevel.Debug();

                    log.WriteTo.Console();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<SubmitClaimConsumer>();

                        x.UsingGrpc((context, cfg) =>
                        {
                            var options = context.GetRequiredService<IOptions<StartupOptions>>();
                            cfg.Host(h =>
                            {
                                if (!string.IsNullOrWhiteSpace(options.Value.Host))
                                    h.Host = options.Value.Host;

                                if (options.Value.Port.HasValue)
                                    h.Port = options.Value.Port.Value;

                                foreach (var host in options.Value.GetServers())
                                    h.AddServer(host);
                            });

                            //                            if (string.IsNullOrWhiteSpace(options.Value.Servers))
                            cfg.ConfigureEndpoints(context, filter => filter.Include<SubmitClaimConsumer>());
                        });
                    });

                    services.AddMassTransit<ISecondBus>(x =>
                    {
                        x.UsingGrpc((context, cfg) =>
                        {
                            var options = context.GetRequiredService<IOptions<StartupOptions>>();
                            cfg.Host(h =>
                            {
                                if (!string.IsNullOrWhiteSpace(options.Value.Host))
                                    h.Host = options.Value.Host;

                                if (options.Value.Port.HasValue)
                                    h.Port = options.Value.Port.Value + 10000;

                                h.AddServer(new Uri("http://127.0.0.1:19796"));
                            });
                        });
                    });

                    services.AddMassTransitHostedService(true);
                    services.AddHostedService<ClaimSubmissionService>();

                    services.AddOptions<StartupOptions>()
                        .Configure<IConfiguration>((options, config) =>
                        {
                            config.Bind(options);
                        });
                });
        }
    }


    public interface ISecondBus :
        IBus
    {
    }
}