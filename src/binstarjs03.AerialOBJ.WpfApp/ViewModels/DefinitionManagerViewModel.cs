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
    private readonly IDefinitionManagerService _definitionManager;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    private ViewportDefinition _selectedDefinition;

    public DefinitionManagerViewModel(IDefinitionManagerService definitionManager,
                                      IModalService modalService,
                                      ILogService logService)
    {
        _definitionManager = definitionManager;
        _modalService = modalService;
        _logService = logService;
        _selectedDefinition = definitionManager.CurrentViewportDefinition;
    }

    public ObservableCollection<ViewportDefinition> LoadedViewportDefinitions => _definitionManager.LoadedViewportDefinitions;
    public bool CanDeleteDefinition => SelectedDefinition != _definitionManager.DefaultViewportDefinition;

    [RelayCommand]
    private void ImportDefinition()
    {
        if (!isDefinitionFileConfirmedFromFileDialog(out string path))
            return;
        try
        {
            _definitionManager.ImportDefinitionFile(path);
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
    private void DeleteDefinition()
    {
        bool result = _modalService.ShowConfirmationBox(new MessageBoxArg()
        {
            Caption = "Confirm Deleting Definition",
            Message = $"Are you sure to delete definition {SelectedDefinition.Name}?\n" +
                       "This cannot be undone"
        });
        if (result)
            LoadedViewportDefinitions.Remove(SelectedDefinition);
        SelectedDefinition = _definitionManager.DefaultViewportDefinition;
    }

    [RelayCommand]
    private void Close()
    {
        _definitionManager.CurrentViewportDefinition = SelectedDefinition;
    }
}
