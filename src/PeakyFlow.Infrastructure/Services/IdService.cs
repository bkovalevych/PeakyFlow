using PeakyFlow.Application.Common.Interfaces;

namespace PeakyFlow.Infrastructure.Services
{
    public class IdService : IGuid
    {
        public string NewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
