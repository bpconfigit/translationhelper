using System;
using System.Windows.Input;

namespace TranslationHelper.Command {
  public abstract class AbstractCommandManagerCommand : ICommand {
    public event EventHandler CanExecuteChanged {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }
    public virtual bool CanExecute( object parameter ) {
      return true;
    }

    public abstract void Execute( object parameter );
  }
}
