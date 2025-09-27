using LeOne.Domain.DomainEvents;
using System.Collections.ObjectModel;

namespace LeOne.Application.Common.DomainEvents;

public interface IDomainEventTypeProvider
{
    bool TryGetEventType(string identifier, out Type eventType);
    DomainEventTypeDescriptor? FindByIdentifier(string identifier);
    DomainEventTypeDescriptor? FindByType(Type eventType);
    IReadOnlyCollection<DomainEventTypeDescriptor> GetAll();
}