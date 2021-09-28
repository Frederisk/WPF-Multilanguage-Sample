using System;
using System.Windows.Input;

namespace MultiLanguage.Common {
    /// <summary>
    /// Bind actions to the view as commands
    /// </summary>
    public class DelegateCommand : ICommand {
        private readonly Predicate<Object?>? _canExecute;
        private readonly Action<Object?> _execute;

        public DelegateCommand(Action<Object?> execute, Predicate<Object?>? canExecute = null) {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(Object? parameter) {
            return this._canExecute is null || this._canExecute(parameter);
        }

        public void Execute(Object? parameter) {
            this._execute(parameter);
        }
        //public void RaiseCanExecuteChanged() {
        //    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}

        public event EventHandler? CanExecuteChanged {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove { 
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
