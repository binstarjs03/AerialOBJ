using System;
using System.Windows.Input;

namespace binstarjs03.AerialOBJ.WpfApp;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;
    private readonly object? _arg;

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null, object? arg = null)
    {
        _execute = execute;
        _canExecute = canExecute;
        _arg = arg;
    }

    public bool CanExecute(object? parameter)
    {
        if (_canExecute is null)
            return true;
        return _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        if (_arg is not null)
            _execute(_arg);
        else
            _execute(parameter);
    }
}
