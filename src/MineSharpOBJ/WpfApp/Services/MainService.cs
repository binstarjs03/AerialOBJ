namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public static class MainService {
    public static void Initialize() {
        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        LogService.Log("Initialization complete", useSeparator: true);
    }  
}
