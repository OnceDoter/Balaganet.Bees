using Engine.Field.Events;

namespace Engine.Field;

public abstract record EntityBase
{
    protected internal void RaiseEvents(params DomainEventType[] events)
    {
        foreach (var @event in events)
        {
            DomainEvent.Holder.Add(new DomainEvent { Type = @event, Source = this });
        }
    }
}