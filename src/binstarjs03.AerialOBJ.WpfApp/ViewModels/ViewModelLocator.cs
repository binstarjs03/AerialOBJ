using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public class ViewModelLocator
{
    public ViewModelLocator()
    {
        AbstractViewModel = App.Current?.ServiceProvider.GetRequiredService<AbstractViewModel>()!;
    }

    public AbstractViewModel AbstractViewModel { get; }
}
