using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class DefinitionManagerViewModel
{
    private readonly DefinitionManagerService _definitionManager;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(CanDeleteDefinition))] private ViewportDefinition _selectedDefinition;

    public DefinitionManagerViewModel(DefinitionManagerService definitionManager, IModalService modalService, ILogService logService)
    {
        _definitionManager = definitionManager;
        _modalService = modalService;
        _logService = logService;
        _selectedDefinition = definitionManager.CurrentViewportDefinition;
    }

    public ObservableCollection<ViewportDefinition> ViewportDefinitions => _definitionManager.ViewportDefinitions;
    public bool CanDeleteDefinition => SelectedDefinition != _definitionManager.DefaultViewportDefinition;

    [RelayCommand]
    private void OnImportDefinition()
    {
        FileDialogResult result = _modalService.ShowOpenFileDialog(new FileDialogArg()
        {
            FileExtension = ".json",
            FileExtensionFilter = "JSON File|*.json",
            FileName = ""
        });
        if (!result.Result)
            return;
        string input = File.ReadAllText(result.SelectedFilePath);
        try
        {
            ViewportDefinition? definition = JsonSerializer.Deserialize<ViewportDefinition>(input);
            if (definition is null)
                throw new NullReferenceException("Definition is null");
            ViewportDefinitions.Add(definition);
        }
        catch (JsonException e)
        {
            string msg = "Cannot import definition because the JSON structure mismatched.\n" +
                "See \"Debug Log\" window for more details";
            handleError(msg, e);
        }
        catch (Exception e)
        {
            string msg = "Cannot import definition because of unhandled exception.\n" +
                "See \"Debug Log\" window for more details";
            handleError(msg, e);
        }

        void handleError(string message, Exception e)
        {
            _logService.Log(message, LogStatus.Error);
            _logService.Log(e.Message);
            _logService.Log("Exception Details:");
            _logService.Log(e.ToString());
            _logService.Log("Importing definition aborted", LogStatus.Aborted, true);
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Error Importing Definition",
                Message = message,
            });
        }
    }

    [RelayCommand]
    private void OnDeleteDefinition()
    {
        bool result = _modalService.ShowConfirmationDialog(new MessageBoxArg()
        {
            Caption = "Confirm Deleting Definition",
            Message = $"Are you sure to delete definition {SelectedDefinition.Name}?\n" +
                       "This cannot be undone"
        });
        if (result)
            ViewportDefinitions.Remove(SelectedDefinition);
        SelectedDefinition = _definitionManager.DefaultViewportDefinition;
    }
}
