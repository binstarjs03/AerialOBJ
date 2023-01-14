using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class DefinitionManagerService : IDefinitionManagerService
{
    private readonly ViewportDefinition _defaultViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    private readonly IViewportDefinitionLoader _viewportDefinitionLoaderService;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private ViewportDefinition _currentDefinition;

    public DefinitionManagerService(IViewportDefinitionLoader viewportDefinitionLoaderService,
                                    ILogService logService,
                                    IModalService modalService)
    {
        LoadedViewportDefinitions = new ObservableCollection<ViewportDefinition>
        {
            DefaultViewportDefinition,
        };
        _currentDefinition = DefaultViewportDefinition;
        _viewportDefinitionLoaderService = viewportDefinitionLoaderService;
        _logService = logService;
        _modalService = modalService;
    }

    public ViewportDefinition DefaultViewportDefinition => _defaultViewportDefinition;
    public ViewportDefinition CurrentViewportDefinition
    {
        get => _currentDefinition;
        set
        {
            if (value == _currentDefinition)
                return;
            OnViewportDefinitionChanging?.Invoke();
            _currentDefinition = value;
            OnViewportDefinitionChanged?.Invoke();
        }

    }
    public ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; private set; }

    public event Action? OnViewportDefinitionChanging;
    public event Action? OnViewportDefinitionChanged;

    public void ImportDefinitionFile(string path)
    {
        ViewportDefinition definition = _viewportDefinitionLoaderService.ImportDefinitionFile(path);
        LoadViewportDefinition(definition);
    }

    public void LoadViewportDefinition(ViewportDefinition definition)
    {
        LoadedViewportDefinitions.Add(definition);
    }

    public void LoadDefinitionFolder()
    {
        bool hasErrorMessageBoxShown = false;
        List<ViewportDefinition> loadedDefinitions = _viewportDefinitionLoaderService.LoadDefinitionFolder(exceptionHandler);
        void exceptionHandler(Exception e, string definitionFileName)
        {
            string caption = "Cannot load definition";
            _logService.LogException($"{caption} {definitionFileName}", e);
            if (!hasErrorMessageBoxShown)
                _modalService.ShowErrorMessageBox(new MessageBoxArg()
                {
                    Caption = caption,
                    Message = $"An exception occured during loading definition {definitionFileName}." +
                              $"Any further exception during definition folder loading will be logged to Debug Log window instead"
                });
            hasErrorMessageBoxShown = true;
        }
        foreach (ViewportDefinition definition in loadedDefinitions)
            LoadViewportDefinition(definition);
    }
}
