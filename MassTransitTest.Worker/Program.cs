using MassTransit;
using MassTransitTest.Core;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {

        var connectionString = builder.Configuration.GetConnectionString("Default");

        services.AddDbContext<TestDbContext>(x =>
        {

            x.UseNpgsql(connectionString, options =>
            {
                options.MinBatchSize(1);
            });

            x.UseSnakeCaseNamingConvention();
        });

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<TestDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();

                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
            });

            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<RegistrationSubmittedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();
