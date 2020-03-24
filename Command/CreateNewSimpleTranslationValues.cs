using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Model;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class CreateNewSimpleTranslationValues : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      return translationModel?.CurrentSelected != null && translationModel?.CurrentSelected.Values.Count == 0;
    }

    public override void Execute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      if ( translationModel == null ) {
        return;
      }

      foreach ( var cultureInfo in translationModel.AvailableCultures.Cultures ) {
        translationModel.CurrentSelected.Values.Add( new Value { Lang = cultureInfo.Lang } );
      }
    }
  }
}
