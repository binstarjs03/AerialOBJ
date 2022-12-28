using binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public interface IMessageDispatcherFactory
{
    IMessageDispatcher Create(string name, ExceptionBehaviour exceptionBehaviour);
}