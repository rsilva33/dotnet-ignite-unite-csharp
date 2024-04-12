using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure.Contexts;

namespace PassIn.Application.UseCases.Events.Attendees.GetAllByEventId;
public class GetAllAttendeesByEventIdUseCase
{
    private readonly PassInDbContext _context;

    public GetAllAttendeesByEventIdUseCase() => _context = new PassInDbContext();

    public ResponseAllAttendeesJson Execute(Guid eventId)
    {
        var entity = _context
                        .Events
                        .Include(ev => ev.Attendees)
                        .ThenInclude(at => at.CheckIn)
                        .FirstOrDefault(ev => ev.Id == eventId); //_context.Attendees.Where(at => at.Event_Id == eventId).ToList();

        if (entity is null)
            throw new NotFoundException("An event with this id does not exist.");

        return new ResponseAllAttendeesJson
        {
            Attendees = entity.Attendees.Select(attendee => new ResponseAttendeeJson
            {
                Id = attendee.Id,
                Name = attendee.Name,
                Email = attendee.Email,
                CreatedAt = attendee.Created_At,
                CheckedInAt = attendee.CheckIn?.Created_At
            }).ToList(),
        };
    }
}
