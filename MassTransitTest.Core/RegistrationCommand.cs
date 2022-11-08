using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MassTransitTest.Core;


public class RegistrationCommand : IRequestHandler<RegistrationRequest, RegistrationResponse>
{
    private readonly TestDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegistrationCommand(TestDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<RegistrationResponse> Handle(RegistrationRequest request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var registrationExists = await _context.Registrations
            .Where(x => x.MemberId == request.MemberId)
            .Where(x => x.EventId == request.EventId)
            .AnyAsync();

        if (registrationExists)
        {
            throw new ApplicationException("Registration already exists");
        }

        var registration = new Registration
        {
            RegistrationDate = DateTime.UtcNow,
            MemberId = request.MemberId,
            EventId = request.EventId,
            Payment = request.Payment
        };

        await _context.Registrations.AddAsync(registration);

        await _context.SaveChangesAsync();

        var message = new RegistrationSubmitted
        {
            RegistrationId = registration.Id,
            RegistrationDate = registration.RegistrationDate,
            MemberId = registration.MemberId,
            EventId = registration.EventId,
            Payment = registration.Payment
        };

        await _publishEndpoint.Publish(message, cancellationToken);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return new();
    }
}

public record RegistrationRequest(string MemberId, string EventId, decimal Payment) : IRequest<RegistrationResponse>;

public record RegistrationResponse;

