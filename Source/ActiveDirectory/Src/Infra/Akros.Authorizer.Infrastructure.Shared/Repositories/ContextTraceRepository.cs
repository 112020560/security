using Akros.Authorizer.Application.Repositories;

namespace Akros.Authorizer.Infrastructure.Shared.Repositories
{
    public sealed class ContextTraceRepository: IContextTraceRepository
    {
        public Guid GetTraceId() => Guid.NewGuid();
    }
}
