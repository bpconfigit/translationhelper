using System;

namespace TranslationHelper.Command {
  public class ApplicationExitCommand : AbstractCommandManagerCommand {
    public override void Execute( object parameter ) {
      Environment.Exit( 0 );
    }
  }
}
