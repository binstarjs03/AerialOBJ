using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Factories;
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
    private readonly IFileInfoFactory _fileFactory;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly IFileUtilsService _fileUtilsService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    private ViewportDefinition _selectedDefinition;

    public DefinitionManagerViewModel(DefinitionManagerService definitionManager,
                                      GlobalState globalState,
                                      IFileInfoFactory fileFactory,
                                      IModalService modalService,
                                      ILogService logService,
                                      IFileUtilsService fileUtilsService)
    {
        _definitionManager = definitionManager;
        _globalState = globalState;
        _fileFactory = fileFactory;
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
            string input = _fileUtilsService.ReadAllText(result.SelectedFilePath);
            ViewportDefinition definition = deserialize(input);
            copyFileToDefinitionFolder(result.SelectedFilePath);
            ViewportDefinitions.Add(definition);
        }
        catch (Exception e) { handleError(e); }

        void handleError(Exception e)
        {
            string msg = $"Cannot import definition:\n" +
                         $"{e.Message}\n" +
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

        static ViewportDefinition deserialize(string input)
        {
            ViewportDefinition? definition = JsonSerializer.Deserialize<ViewportDefinition>(input);
            if (definition is null)
                throw new NullReferenceException("Definition is null");
            return definition;
        }

        void copyFileToDefinitionFolder(string path)
        {
            IFileInfo originalFile = _fileFactory.Create(path);
            string targetDirectory = $"{_globalState.CurrentPath}\\Definitions";
            string targetCopyPath = $"{targetDirectory}\\{originalFile.Name}";
            _fileUtilsService.CreateDirectory(targetDirectory);
            _fileUtilsService.Copy(path, targetCopyPath);
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
