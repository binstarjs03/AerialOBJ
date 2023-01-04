using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class DefinitionManagerViewModel
{
    private readonly DefinitionManagerService _definitionManager;
    private readonly GlobalState _globalState;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly IViewportDefinitionLoaderService _viewportDefinitionLoaderService;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    private ViewportDefinition _selectedDefinition;

    public DefinitionManagerViewModel(DefinitionManagerService definitionManager,
                                      GlobalState globalState,
                                      IModalService modalService,
                                      ILogService logService,
                                      IViewportDefinitionLoaderService viewportDefinitionLoaderService)
    {
        _definitionManager = definitionManager;
        _globalState = globalState;
        _modalService = modalService;
        _logService = logService;
        _viewportDefinitionLoaderService = viewportDefinitionLoaderService;
        _selectedDefinition = definitionManager.CurrentViewportDefinition;
    }

    public ObservableCollection<ViewportDefinition> ViewportDefinitions => _definitionManager.ViewportDefinitions;
    public bool CanDeleteDefinition => SelectedDefinition != _definitionManager.DefaultViewportDefinition;

    [RelayCommand]
    private void OnImportDefinition()
    {
        if (!isDefinitionFileConfirmedFromFileDialog(out string path))
            return;
        try
        {
            ViewportDefinition definition = _viewportDefinitionLoaderService.ImportDefinitionFile(path);
            ViewportDefinitions.Add(definition);
        }
        catch (Exception e) { handleException(e); }

        bool isDefinitionFileConfirmedFromFileDialog(out string path)
        {
            FileDialogResult dialogResult = _modalService.ShowOpenFileDialog(new FileDialogArg()
            {
                FileExtension = ".json",
                FileExtensionFilter = "JSON File|*.json",
                FileName = ""
            });
            path = dialogResult.SelectedFilePath;
            return dialogResult.Result;
        }

        void handleException(Exception e)
        {
            _logService.LogException("Cannot import definition", e, "Importing definition aborted");
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Error Importing Definition",
                Message = e.Message,
            });
        }
    }

    [RelayCommand]
    private void OnDeleteDefinition()
    {
        bool result = _modalService.ShowConfirmationBox(new MessageBoxArg()
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
