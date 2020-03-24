using System;

namespace TranslationHelper.Command {
  public class DelegateCommand : AbstractCommandManagerCommand {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;

    public DelegateCommand( Predicate<object> canExecute, Action<object> execute ) {
      _canExecute = canExecute;
      _execute = execute;
    }

    public override bool CanExecute( object parameter ) {
      return base.CanExecute( parameter ) && _canExecute( parameter );
    }

    public override void Execute( object parameter ) {
      _execute( parameter );
    }
  }
}
