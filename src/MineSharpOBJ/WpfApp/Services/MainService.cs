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
        LogService.Log($"Launch time: {DateTime.Now}");
        LogService.Log("MineSharpOBJ v0.0");
        LogService.Log("Commit Hash: 36fa86b5d8eda9bf9a4ea798bfffc3fc07c965e0", useSeparator: true);

        LogService.Log("Starting Initialization...");
        LogService.Log("Initialization complete", useSeparator: true);
    }
}
