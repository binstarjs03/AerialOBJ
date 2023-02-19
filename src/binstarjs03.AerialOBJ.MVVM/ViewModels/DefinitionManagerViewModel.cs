using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.MVVM.Models.Settings;
using binstarjs03.AerialOBJ.MVVM.Repositories;
using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.IOService;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;

public class DefinitionManagerViewModel
{
    private readonly Setting _setting;
    private readonly IDefinitionRepository _definitionRepo;
    private readonly IDefinitionIO _definitionIO;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;

    private DefinitionKinds _context;

    private ViewportDefinition _selectedViewportDefinition;

    public DefinitionManagerViewModel(Setting setting,
                                      IDefinitionRepository definitionRepo,
                                      IDefinitionIO definitionIO,
                                      IModalService modalService,
                                      ILogService logService)
    {
        _setting = setting;
        _definitionRepo = definitionRepo;
        _definitionIO = definitionIO;
        _modalService = modalService;
        _logService = logService;

        _context = DefinitionKinds.Viewport;
        _selectedViewportDefinition = setting.DefinitionSetting.CurrentViewportDefinition;
    }
}
