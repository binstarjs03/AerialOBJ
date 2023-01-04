using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp;
public class AppResourceBootstrapper
{
	public AppResourceBootstrapper()
	{
		AbstractViewModel = App.Current?.ServiceProvider.GetRequiredService<AbstractViewModel>()!;
	}

	public AbstractViewModel AbstractViewModel { get; }
}
