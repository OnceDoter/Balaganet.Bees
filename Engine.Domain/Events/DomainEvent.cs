using System.Collections.Concurrent;

namespace Engine.Field.Events;

public class DomainEvent
{
    public static ConcurrentBag<DomainEvent> Holder = [];
    
    public long Id { get; set; }
    
    public DomainEventType Type { get; set; }
    public object Source { get; set; }
    public DomainEventState State { get; set; } = DomainEventState.Created;

    public void MarkAsHandled()
    {
        State = DomainEventState.Handled;
    }
}