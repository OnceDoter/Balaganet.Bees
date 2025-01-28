namespace Engine.Field.Events;

public enum DomainEventState: byte
{
    Created = 0,
    Handled = 1,
    Unhandled = 2,
}