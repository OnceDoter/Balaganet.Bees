using Engine.Field.Events;
using Engine.Field.Map;

namespace Engine.App;

public class DomainEventHandler : IHandler<DomainEvent>
{
    public void Handle(DomainEvent @event)
    {
        switch (@event.Type)
        {
            case DomainEventType.DimensionCreated:
                DimensionGenerator.GenerateDimension((Dimension)@event.Source);
                @event.MarkAsHandled();
                break;
            case DomainEventType.DimensionPartCreated:
                DimensionPartGenerator.GenerateDimensionPart((DimensionPart)@event.Source);
                @event.MarkAsHandled();
                break;
        }
    }
}