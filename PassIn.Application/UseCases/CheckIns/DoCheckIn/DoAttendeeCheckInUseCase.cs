using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure.Contexts;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.CheckIns.DoCheckIn;
public class DoAttendeeCheckInUseCase
{
    private readonly PassInDbContext _context;

    public DoAttendeeCheckInUseCase() => _context = new PassInDbContext();

    public ResponseRegisteredJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);

        var entity = new CheckIn
        {
            Attendee_Id = attendeeId,
            Created_At = DateTime.UtcNow,
        };

        _context.CheckIns.Add(entity);
        _context.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid attendeeId)
    {
        var existAttendee = _context.Attendees.Any(at => at.Id == attendeeId);

        if (existAttendee is false)
            throw new NotFoundException("The attendee with thsi Id was not found.");

        var existsCheckIn = _context.CheckIns.Any(ch => ch.Attendee_Id == attendeeId);

        if (existsCheckIn)
            throw new ConflictException("Attendee can not do checking twice in the same event.");
    }
}
