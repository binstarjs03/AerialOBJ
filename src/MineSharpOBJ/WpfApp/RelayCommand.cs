using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace binstarjs03.MineSharpOBJ.WpfApp;
public class RelayCommand : ICommand {
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public event EventHandler? CanExecuteChanged {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) {
        if (_canExecute == null)
            return true;
        return _canExecute(parameter);
    }

    public void Execute(object? parameter) {
        _execute(parameter);
    }
}
