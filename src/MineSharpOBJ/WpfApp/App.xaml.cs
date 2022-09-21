using System;
using System.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp;

public partial class App : Application {
	public App() {
		LauchTime = DateTime.Now;
	}

    public static DateTime LauchTime { get; set; }
}
