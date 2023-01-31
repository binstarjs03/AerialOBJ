using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

public enum DefinitionContext
{
    Viewport = 0,
}

[ObservableObject]
public partial class DefinitionManagerViewModel
{
    private readonly DefinitionSetting _definitionSetting;
    private readonly IDefinitionManager _definitionManager;
    private readonly IDefinitionIO _definitionIO;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContextDefinition))]
    [NotifyPropertyChangedFor(nameof(ContextLoadedDefinitions))]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    [NotifyCanExecuteChangedFor(nameof(DeleteDefinitionCommand))]
    private DefinitionContext _context;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
    [NotifyCanExecuteChangedFor(nameof(DeleteDefinitionCommand))]
    private ViewportDefinition _selectedViewportDefinition;

    public DefinitionManagerViewModel(IDefinitionManager definitionManager,
                                      IModalService modalService,
                                      ILogService logService,
                                      IDefinitionIO definitionIO,
                                      Setting setting)
    {
        _definitionManager = definitionManager;
        _modalService = modalService;
        _logService = logService;
        _definitionIO = definitionIO;
        _definitionSetting = setting.DefinitionSetting;

        _context = DefinitionContext.Viewport;
        _selectedViewportDefinition = _definitionSetting.CurrentViewportDefinition;
    }

    public ObservableCollection<ViewportDefinition> LoadedViewportDefinitions => _definitionManager.LoadedViewportDefinitions;

    public IRootDefinition ContextDefinition
    {
        get
        {
            return Context switch
            {
                DefinitionContext.Viewport => SelectedViewportDefinition,
                _ => throw new NotImplementedException(),
            };
        }
        set
        {
            if (Context == DefinitionContext.Viewport)
            {
                value ??= DefinitionSetting.DefaultViewportDefinition;
                SelectedViewportDefinition = (value as ViewportDefinition)!;
            }
            else
                throw new NotImplementedException();
            OnPropertyChanged();
        }
    }

    // we cannot set return value to ObservableCollection<IRootDefinition>, so set it to object instead
    public object ContextLoadedDefinitions => Context switch
    {
        DefinitionContext.Viewport => LoadedViewportDefinitions,
        _ => throw new NotImplementedException(),
    };

    public bool CanDeleteDefinition => !ContextDefinition.IsDefault;

    [RelayCommand]
    private void ImportDefinition()
    {
        if (!isDefinitionFileConfirmedFromFileDialog(out string path))
            return;
        try
        {
            IRootDefinition definition = _definitionIO.ImportDefinition(path);
            _definitionManager.LoadDefinition(definition);
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
            return dialogResult.Confirmed;
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

    [RelayCommand(CanExecute = nameof(CanDeleteDefinition))]
    private void DeleteDefinition()
    {
        //IRootDefinition definition = Conte
        bool result = _modalService.ShowWarningConfirmationBox(new MessageBoxArg()
        {
            Caption = "Confirm Deleting Definition",
            Message = $"Are you sure to delete definition \"{ContextDefinition.Name}\"?\n" +
                       "This cannot be undone"
        });
        if (result)
        {
            IRootDefinition deletedDefinition = ContextDefinition;
            _definitionManager.UnloadDefinition(deletedDefinition, _definitionSetting);
            _definitionIO.DeleteDefinition(deletedDefinition);
        }
    }

    [RelayCommand]
    private void OnClosing()
    {
        _definitionSetting.CurrentViewportDefinition = SelectedViewportDefinition;
    }
}
