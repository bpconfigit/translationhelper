using TranslationHelper.Model;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class DeleteSingleTranslationValue : AbstractCommandManagerCommand {
    public TranslationResources TranslationModel { get; set; }

    public override bool CanExecute( object parameter ) {
      return TranslationModel?.CurrentSelected != null;
    }

    public override void Execute( object parameter ) {
      var value = parameter as Value;
      TranslationModel.CurrentSelected.Values.Remove( value );
    }
  }
}
