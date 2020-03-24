using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class OpenSearchCommand : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var appModel = parameter as ApplicationModel;
      return appModel?.TranslationDirectory != null;
    }

    public override void Execute( object parameter ) {
      var appModel = parameter as ApplicationModel;
      if ( appModel.SearchDialog != null ) {
        if ( !appModel.SearchDialog.IsVisible ) {
          appModel.SearchDialog.Show();
          return;
        }
        appModel.SearchDialog.Activate();
        return;
      }
      var dialog = new SearchPage();
      dialog.Closed += ( sender, e ) => appModel.SearchDialog = null;
      dialog.Show();
      appModel.SearchDialog = dialog;
    }
  }
}
