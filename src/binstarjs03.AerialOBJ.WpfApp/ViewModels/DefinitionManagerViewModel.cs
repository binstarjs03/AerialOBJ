using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Components;
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
    private readonly IFileUtilsService _fileUtilsService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    private ViewportDefinition _selectedDefinition;

    public DefinitionManagerViewModel(DefinitionManagerService definitionManager,
                                      IModalService modalService,
                                      ILogService logService,
                                      IFileUtilsService fileUtilsService)
    {
        _definitionManager = definitionManager;
        _modalService = modalService;
        _logService = logService;
        _fileUtilsService = fileUtilsService;
        _selectedDefinition = definitionManager.CurrentViewportDefinition;
    }

    public ObservableCollection<ViewportDefinition> ViewportDefinitions => _definitionManager.ViewportDefinitions;
    public bool CanDeleteDefinition => SelectedDefinition != _definitionManager.DefaultViewportDefinition;

    [RelayCommand]
    private void OnImportDefinition()
    {
        FileDialogResult result = showDialog();
        if (!result.Result)
            return;
        try
        {
            checkFileExistence(result);
            string input = _fileUtilsService.ReadAllText(result.SelectedFilePath);
            ViewportDefinition definition = deserialize(input);
            ViewportDefinitions.Add(definition);
        }
        catch (Exception e) { handleError(e); }

        void handleError(Exception e)
        {
            string reason;
            if (e is FileNotFoundException)
                reason = "the file do not exist in given path";
            else if (e is JsonException)
                reason = "the JSON structure mismatched";
            else
                reason = "of unhandled exception";
            string msg = $"Cannot import definition because {reason}.\n" +
                          "See \"Debug Log\" window for more details";
            _logService.Log(msg, LogStatus.Error);
            _logService.Log(e.Message);
            _logService.Log("Exception Details:");
            _logService.Log(e.ToString());
            _logService.Log("Importing definition aborted", LogStatus.Aborted, true);
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Error Importing Definition",
                Message = msg,
            });
        }

        FileDialogResult showDialog()
        {
            return _modalService.ShowOpenFileDialog(new FileDialogArg()
            {
                FileExtension = ".json",
                FileExtensionFilter = "JSON File|*.json",
                FileName = ""
            });
        }

        void checkFileExistence(FileDialogResult result)
        {
            if (!_fileUtilsService.Exist(result.SelectedFilePath))
                throw new FileNotFoundException("File do not exist in given path", result.SelectedFilePath);
        }

        static ViewportDefinition deserialize(string input)
        {
            ViewportDefinition? definition = JsonSerializer.Deserialize<ViewportDefinition>(input);
            if (definition is null)
                throw new NullReferenceException("Definition is null");
            return definition;
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
