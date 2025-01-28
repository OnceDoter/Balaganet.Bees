namespace Engine.App;

public interface IHandler<in TArg0>
{
    public void Handle(TArg0 @event);
}