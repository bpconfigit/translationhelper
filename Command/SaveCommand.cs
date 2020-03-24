using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class SaveCommand : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      return translationModel?.Changed == true;
    }

    public override void Execute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      translationModel?.SaveActive();
    }
  }
}
