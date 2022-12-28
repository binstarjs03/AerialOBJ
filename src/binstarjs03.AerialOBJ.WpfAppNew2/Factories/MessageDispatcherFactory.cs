using binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public class MessageDispatcherFactory : IMessageDispatcherFactory
{
    public IMessageDispatcher Create(string name, ExceptionBehaviour exceptionBehaviour)
    {
        return new MessageDispatcher()
        {
            Name = name,
            ExceptionBehaviour = exceptionBehaviour
        };
    }
}
