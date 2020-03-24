using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class CreateNewTranslation : AbstractCommandManagerCommand {
    public override void Execute( object parameter ) {
      var translationModel = parameter as TranslationResources;

      translationModel?.CreateNewTranslation();
    }
  }
}
