using MassTransit;

namespace MassTransitTest.Core;

public class RegistrationSubmittedConsumer : IConsumer<RegistrationSubmitted>
{
    public Task Consume(ConsumeContext<RegistrationSubmitted> context)
    {
        Console.WriteLine($"User registered {context.Message.RegistrationId} at {context.Message.RegistrationDate}");

        return Task.CompletedTask;
    }
}

public record RegistrationSubmitted
{
    public int RegistrationId { get; init; }
    public DateTime RegistrationDate { get; init; }
    public string MemberId { get; init; } = null!;
    public string EventId { get; init; } = null!;
    public decimal Payment { get; init; }
}