using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure.Contexts;
using PassIn.Infrastructure.Entities;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;
public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _context;

    public RegisterAttendeeOnEventUseCase() => _context = new PassInDbContext();
    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);

        var entity = CreateAttendee(eventId, request);

        _context.Attendees.Add(entity);
        _context.SaveChanges();

        return new ResponseRegisteredJson { Id = entity.Id };

    }

    private static Attendee CreateAttendee(Guid eventId, RequestRegisterEventJson request)
    {
        return new Attendee
        {
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow,
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _context.Events.Find(eventId);

        if (eventEntity is null)
            throw new NotFoundException("An event with this id does not exist.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ErrorOnValidationException("The name is invalid.");

        if (EmailIsValid(request.Email) is false)
            throw new ErrorOnValidationException("the e-mail is invalid.");

        if (AttendeeAlreadyRegisteredd(eventId, request))
            throw new ConflictException("You can not register twice on the same event.");

        var attendeesForEvent = CountAttendeesForEvent(eventId);
        if (attendeesForEvent == eventEntity.Maximum_Attendees)
            throw new ErrorOnValidationException("there is no room for this event.");
    }

    private int CountAttendeesForEvent(Guid eventId)
    {
        var countAttendees = _context
            .Attendees
            .Count(at => at.Event_Id == eventId);

        return countAttendees;
    }

    private bool AttendeeAlreadyRegisteredd(Guid eventId, RequestRegisterEventJson request)
    {
        var attendeeAlreadyregistered = _context
            .Attendees
            .Any(at => at.Email.Equals(request.Email) && at.Event_Id == eventId);

        return attendeeAlreadyregistered;
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
