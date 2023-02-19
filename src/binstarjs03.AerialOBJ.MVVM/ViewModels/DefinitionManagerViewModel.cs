using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.MVVM.Models.Settings;
using binstarjs03.AerialOBJ.MVVM.Repositories;
using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.IOService;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;

public partial class DefinitionManagerViewModel : ObservableObject
{
    //private readonly DefinitionSetting _definitionSetting;
    //private readonly IDefinitionRepository _definitionRepo;
    //private readonly IDefinitionIO _definitionIO;
    //private readonly IModalService _modalService;
    //private readonly ILogService _logService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DataContext))]
    private DefinitionKinds _context;

    private readonly ContextViewModel<ViewportDefinition> _viewportDataContext;
    private readonly ContextViewModel<ModelDefinition> _modelDataContext;

    public DefinitionManagerViewModel(Setting setting, IDefinitionRepository definitionRepo)
    {
        var definitionSetting = setting.DefinitionSetting;
        _context = DefinitionKinds.Viewport;
        _viewportDataContext = new(definitionRepo.ViewportDefinitions,
                                   definitionSetting.CurrentViewportDefinition);
        _modelDataContext = new(definitionRepo.ModelDefinitions,
                                definitionSetting.CurrentModelDefinition);
    }

    public object DataContext => Context switch
    {
        DefinitionKinds.Viewport => _viewportDataContext,
        DefinitionKinds.Model => _modelDataContext,
        _ => throw new System.NotImplementedException(),
    };

    public partial class ContextViewModel<T> : ObservableObject where T : class, IRootDefinition
    {
        [ObservableProperty] private T _selectedDefinition;

        public ContextViewModel(ObservableCollection<T> repository, T selectedDefinition)
        {
            Repository = repository;
            _selectedDefinition = selectedDefinition;
        }

        public ObservableCollection<T> Repository { get; }
    }
}
