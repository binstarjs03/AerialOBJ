using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using binstarjs03.MineSharpOBJ.WpfApp.Views;
using System.Windows.Controls;
using System.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public static class MainService {
    public static void Initialize() {
        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        LogService.Log("Initialization complete", useSeparator: true);
    }  
}
