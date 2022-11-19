using System;
using System.IO;
using System.Windows.Forms;

using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class MainWindowVM : BaseViewModel
{
    [ObservableProperty]
    private string _title = SharedStateService.AppName;

    public bool HasSavegameLoaded => SharedStateService.HasSavegameLoaded;

    public bool IsDebugLogWindowVisible
    {
        get => SharedStateService.IsDebugLogWindowVisible;
        set => SharedStateService.IsDebugLogWindowVisible = value;
    }
    public bool IsViewportSidePanelVisible
    {
        get => SharedStateService.IsViewportSidePanelVisible;
        set => SharedStateService.IsViewportSidePanelVisible = value;
    }
    public bool IsViewportSidePanelDebugInfoVisible
    {
        get => SharedStateService.IsViewportSidePanelDebugInfoVisible;
        set => SharedStateService.IsViewportSidePanelDebugInfoVisible = value;
    }

    public MainWindowVM()
    {
        SharedStateService.SavegameLoadChanged += OnSavegameLoadChanged;
        SharedStateService.ViewportSidePanelVisibilityChanged += OnViewportSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnViewportSidePanelDebugInfoVisibilityChanged;
        SharedStateService.DebugLogWindowVisibilityChanged += OnDebugLogWindowVisibilityChanged;
    }

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        Title = state switch
        {
            SavegameLoadState.Opened => $"{SharedStateService.AppName} - {SharedStateService.SavegameLoadInfo!.WorldName}",
            SavegameLoadState.Closed => SharedStateService.AppName,
            _ => throw new NotImplementedException()
        };
        OnPropertyChanged(nameof(HasSavegameLoaded));
    }
    private void OnDebugLogWindowVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsDebugLogWindowVisible));
    private void OnViewportSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsViewportSidePanelVisible));
    private void OnViewportSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsViewportSidePanelDebugInfoVisible));

    [RelayCommand]
    private void OnOpenSavegame(string? path)
    {
        if (path is null)
        {
            LogService.Log("Attempting to load savegame...");
            using FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                LogService.LogEmphasis("Dialog cancelled. Aborting...", LogService.Emphasis.Aborted);
                return;
            }
            path = dialog.SelectedPath;
        }
        LogService.Log($"Selected path: \"{path}\"");
        LogService.Log($"Loading selected path as Minecraft savegame folder...");
        SavegameLoadInfo loadInfo;
        try
        {
            loadInfo = IOService.LoadSavegame(path, out bool foundRegionFolder);
            if (!foundRegionFolder)
                LogService.LogEmphasis($"Folder \"region\" does not exist in selected path", LogService.Emphasis.Warning);
        }
        catch (LevelDatNotFoundException)
        {
            string msg = "Missing \"level.dat\" file in specified savegame folder.";
            string modalMsg = $"Cannot open \"{path}\" as Minecraft savegame folder: \n"
                            + msg;
            LogService.LogEmphasis(msg, LogService.Emphasis.Error);
            LogService.LogEmphasis("Failed loading savegame. Aborting...",
                                   LogService.Emphasis.Aborted,
                                   useSeparator: true);
            ModalService.ShowErrorOKModal("Error Opening Minecraft Savegame", msg);
            return;
        }
        catch (Exception e)
        {
            string modalMsg = $"Cannot open \"{path}\" as Minecraft savegame folder, "
                            + "Savegame may be corrupted or not supported "
                            + $"by this version of {SharedStateService.AppName}.\n\n"
                            + "Please see the Debug Log Window for more information.";
            string logMsg = $"{modalMsg}\n\n"
                          + "Exception details:\n"
                          + $"{e!.GetType()}: {e.Message}\n{e}";
            LogService.LogEmphasis(logMsg, LogService.Emphasis.Error);
            LogService.LogEmphasis("Failed loading savegame. Aborting...",
                                   LogService.Emphasis.Aborted,
                                   useSeparator: true);
            ModalService.ShowErrorOKModal("Error Opening Minecraft Savegame", modalMsg);
            return;
        }
        // close if session exist, this is to ensure every components are reinitialized
        if (SharedStateService.HasSavegameLoaded)
        {
            LogService.LogEmphasis("Has open Savegame, closing...", LogService.Emphasis.Information);
            OnCloseSavegame(requestedFromOpen: true);
        }
        SharedStateService.SavegameLoadInfo = loadInfo;
        LogService.LogEmphasis($"Successfully changed savegame to \"{loadInfo.WorldName}.\"", LogService.Emphasis.Success, useSeparator: true);
    }

    [RelayCommand]
    private void OnCloseSavegame(bool? requestedFromOpen = false)
    {
        string logSuccessMsg;
        LogService.Log("Attempting to closing savegame...");

        if (SharedStateService.SavegameLoadInfo is null)
        {
            LogService.LogEmphasis($"{nameof(SavegameLoadInfo)} is already closed!", LogService.Emphasis.Warning,
                                   useSeparator: false);
            logSuccessMsg = "Successfully closed savegame.";
        }
        else
        {
            string worldName = SharedStateService.SavegameLoadInfo.WorldName;
            logSuccessMsg = $"Successfully closed savegame \"{worldName}\".";
        }

        SharedStateService.SavegameLoadInfo = null;
        bool useSeparator = requestedFromOpen is null || !requestedFromOpen.Value;
        LogService.LogEmphasis(logSuccessMsg, LogService.Emphasis.Success, useSeparator:useSeparator);
    }

    [RelayCommand]
    private void OnShowAboutModal() => ModalService.ShowAboutModal();
}
