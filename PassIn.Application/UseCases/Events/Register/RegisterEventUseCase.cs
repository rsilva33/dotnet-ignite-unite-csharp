using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure.Contexts;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.Register;
public class RegisterEventUseCase
{
    private readonly PassInDbContext _context;
    public RegisterEventUseCase() => _context = new PassInDbContext();

    public ResponseRegisteredJson Execute(RequestEventJson request)
    {
        Validate(request);

        var entity = CreateEvent(request);

        _context.Events.Add(entity);
        _context.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id
        };
    }

    private static Event CreateEvent(RequestEventJson request)
    {
        return new Event
        {
            Title = request.Title,
            Details = request.Details,
            Maximum_Attendees = request.MaximumAttendees,
            Slug = request.Title.ToLower().Replace(" ", "-"),
        };
    }

    private void Validate(RequestEventJson request)
    {
        if (request.MaximumAttendees <= 0)
            throw new ErrorOnValidationException("The Maximum attendees is invalid.");

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ErrorOnValidationException("The title is invalid.");

        if (string.IsNullOrWhiteSpace(request.Details))
            throw new ErrorOnValidationException("The details is invalid.");
    }
}
