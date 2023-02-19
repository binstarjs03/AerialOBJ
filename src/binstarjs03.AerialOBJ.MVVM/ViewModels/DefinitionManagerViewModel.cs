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
    private readonly DefinitionSetting _definitionSetting;
    private readonly IDefinitionRepository _definitionRepo;
    private readonly IDefinitionIO _definitionIO;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ContextRepository))]
    private DefinitionKinds _context;

    private ViewportDefinition _selectedViewportDefinition;

    public DefinitionManagerViewModel(Setting setting,
                                      IDefinitionRepository definitionRepo,
                                      IDefinitionIO definitionIO,
                                      IModalService modalService,
                                      ILogService logService)
    {
        _definitionSetting = setting.DefinitionSetting;
        _definitionRepo = definitionRepo;
        _definitionIO = definitionIO;
        _modalService = modalService;
        _logService = logService;

        _context = DefinitionKinds.Viewport;
        _selectedViewportDefinition = setting.DefinitionSetting.CurrentViewportDefinition;
    }

    public object ContextRepository => Context switch
    {
        DefinitionKinds.Viewport => _definitionRepo.ViewportDefinitions,
        DefinitionKinds.Model => _definitionRepo.ModelDefinitions,
        _ => throw new System.NotImplementedException(),
    };
}
