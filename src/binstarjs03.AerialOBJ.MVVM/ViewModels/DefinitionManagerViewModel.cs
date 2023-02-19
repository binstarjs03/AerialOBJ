using System;
using System.Collections.ObjectModel;
using System.Linq;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.MVVM.Models.Settings;
using binstarjs03.AerialOBJ.MVVM.Repositories;
using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.IOService;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;

public partial class DefinitionManagerViewModel : ObservableObject
{
    private readonly IDefinitionIO _definitionIO;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;

    private readonly ContextViewModel<ViewportDefinition> _viewportDataContext;
    private readonly ContextViewModel<ModelDefinition> _modelDataContext;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DataContext))]
    private DefinitionKinds _context;

    public DefinitionManagerViewModel(Setting setting, IDefinitionRepository definitionRepo, IModalService modalService, ILogService logService, IDefinitionIO definitionIO)
    {
        _modalService = modalService;
        _logService = logService;
        _definitionIO = definitionIO;

        var definitionSetting = setting.DefinitionSetting;

        _viewportDataContext = new(definitionRepo.ViewportDefinitions,
                                   definitionSetting.CurrentViewportDefinition,
                                   modalService,
                                   definitionIO);
        _modelDataContext = new(definitionRepo.ModelDefinitions,
                                definitionSetting.CurrentModelDefinition,
                                modalService,
                                definitionIO);

        _context = DefinitionKinds.Viewport;
    }

    public object DataContext => Context switch
    {
        DefinitionKinds.Viewport => _viewportDataContext,
        DefinitionKinds.Model => _modelDataContext,
        _ => throw new NotImplementedException(),
    };

    [RelayCommand]
    private void ImportDefinition()
    {
        if (!isDefinitionFileConfirmedFromFileDialog(out string path))
            return;
        try
        {
            IRootDefinition definition = _definitionIO.ImportDefinition(path);
            if (definition is ViewportDefinition viewportDefinition)
                _viewportDataContext.LoadDefinition(viewportDefinition);
            else if (definition is ModelDefinition modelDefinition)
                _modelDataContext.LoadDefinition(modelDefinition);
            else
                throw new NotImplementedException();
        }
        catch (Exception e) { handleException(e); }

        bool isDefinitionFileConfirmedFromFileDialog(out string path)
        {
            FileDialogResult dialogResult = _modalService.ShowOpenFileDialog(new FileDialogArg
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
            _modalService.ShowErrorMessageBox(new MessageBoxArg
            {
                Caption = "Error Importing Definition",
                Message = e.Message,
            });
        }
    }



    public partial class ContextViewModel<T> : ObservableObject where T : class, IRootDefinition
    {
        private readonly IModalService _modalService;
        private readonly IDefinitionIO _definitionIO;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanDeleteDefinition))]
        [NotifyCanExecuteChangedFor(nameof(DeleteDefinitionCommand))]
        private T _selectedDefinition;

        public ContextViewModel(ObservableCollection<T> repository,
                                T selectedDefinition,
                                IModalService modalService,
                                IDefinitionIO definitionIO)
        {
            Repository = repository;
            _selectedDefinition = selectedDefinition;
            _modalService = modalService;
            _definitionIO = definitionIO;
        }

        public ObservableCollection<T> Repository { get; }
        public bool CanDeleteDefinition => !SelectedDefinition.IsDefault;

        public void LoadDefinition(T definition)
        {
            if (!Repository.All(definition => definition.Name != definition.Name))
                throw new ArgumentException("Definition with same name exist");
            Repository.Add(definition);
        }

        [RelayCommand(CanExecute = nameof(CanDeleteDefinition))]
        private void DeleteDefinition()
        {
            bool result = _modalService.ShowWarningConfirmationBox(new MessageBoxArg
            {
                Caption = "Confirm Deleting Definition",
                Message = $"Are you sure to delete definition \"{SelectedDefinition.Name}\"?\n" +
                       "This cannot be undone"
            });
            if (result)
            {
                if (SelectedDefinition.IsDefault)
                    throw new InvalidOperationException("Attempting to delete default definition");

                var deletedDefinition = SelectedDefinition;

                // set selected definition to default
                SelectedDefinition = Repository.Where(item => item.IsDefault).First();
                Repository.Remove(deletedDefinition);
                _definitionIO.DeleteDefinition(deletedDefinition);
            }
        }
    }
}
