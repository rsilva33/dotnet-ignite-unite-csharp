using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure.Contexts;

namespace PassIn.Application.UseCases.Events.GetById;

public class GetEventByIdUseCase
{
    private readonly PassInDbContext _context;

    public GetEventByIdUseCase() => _context = new PassInDbContext();

    public ResponseEventJson Execute(Guid id)
    {
        //var entity = dbContext.Events.FirstOrDefault(ev => ev.Id == id);
        var entity = _context.Events.Find(id);

        if (entity is null)
            throw new NotFoundException("An event with this id does not exist.");

        return new ResponseEventJson
        {
            Id = entity.Id,
            Title = entity.Title,
            Details = entity.Details,
            MaximumAttendees = entity.Maximum_Attendees,
            AttendeesAmount = -1
        };
    }
}