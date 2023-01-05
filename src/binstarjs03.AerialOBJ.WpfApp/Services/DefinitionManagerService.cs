using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class DefinitionManagerService
{
    private readonly ViewportDefinition _defaultViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    private readonly IViewportDefinitionLoaderService _viewportDefinitionLoaderService;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private ViewportDefinition _currentDefinition;

    public DefinitionManagerService(IViewportDefinitionLoaderService viewportDefinitionLoaderService,
                                    ILogService logService,
                                    IModalService modalService)
    {
        ViewportDefinitions = new ObservableCollection<ViewportDefinition>
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
        }

    }
    public ObservableCollection<ViewportDefinition> ViewportDefinitions { get; private set; }

    public event Action? OnViewportDefinitionChanging;

    public void LoadViewportDefinition(ViewportDefinition definition)
    {
        ViewportDefinitions.Add(definition);
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
