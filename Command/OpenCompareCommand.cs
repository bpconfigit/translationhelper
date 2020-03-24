using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class OpenCompareCommand : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var model = parameter as ApplicationModel;
      return model?.TranslationDirectory != null;
    }

    public override void Execute( object parameter ) {
      var cd = new CompareDirectory();
      cd.ShowDialog();
    }
  }
}
