using Ardalis.Specification;
using PeakyFlow.Abstractions.RoomAggregate;

namespace PeakyFlow.Application.Rooms.Specifications
{
    public class FirstOrDefaultRoomSpecification : SingleResultSpecification<Room>
    {
        public FirstOrDefaultRoomSpecification(string id)
        {
            Query.Where(x => x.Id == id)
                .Include(x => x.Players);
        }
    }
}
