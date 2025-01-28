using Engine.Field.Events;

namespace Engine.Field.ExternalEntities;

public abstract class ExternalEntity(ExternalContext context)
{
    protected internal ExternalContext Context => context;
}