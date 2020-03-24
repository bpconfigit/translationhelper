using TranslationHelper.Model;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class CreateNewTranslationValue : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      return translationModel?.CurrentSelected != null;
    }

    public override void Execute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      translationModel.CurrentSelected.Values.Add( new Value() );
    }
  }
}
