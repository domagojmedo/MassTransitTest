using MassTransit;
using MassTransitTest.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<TestDbContext>(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");

    x.UseNpgsql(connectionString, options =>
    {
        options.MinBatchSize(1);
    });

    x.UseSnakeCaseNamingConvention();
});

builder.Services.AddMediatR(typeof(RegistrationCommand));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<RecreateDatabaseHostedService<TestDbContext>>();

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<TestDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);

        o.UsePostgres();
        o.UseBusOutbox(bo => bo.DisableDeliveryService());
        //o.UseBusOutbox();
        o.DisableInboxCleanupService();
    });

    x.RemoveMassTransitHostedService();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
