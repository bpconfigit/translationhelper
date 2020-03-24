using System.Windows.Forms;
using TranslationHelper.Properties;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class OpenCompareDirectoryCommand : AbstractCommandManagerCommand {
    public override void Execute( object parameter ) {
      var appModel = parameter as CompareViewModel;
      if ( appModel == null ) {
        return;
      }
      using ( var fbd = new FolderBrowserDialog() ) {
        if ( !string.IsNullOrWhiteSpace( Settings.Default.LastCompareDirectory ) ) {
          fbd.SelectedPath = Settings.Default.LastCompareDirectory;
        }
        fbd.ShowNewFolderButton = false;
        var result = fbd.ShowDialog();
        if ( result == System.Windows.Forms.DialogResult.OK ) {
          appModel.TargetDirectory = fbd.SelectedPath;
          Settings.Default.LastCompareDirectory = fbd.SelectedPath;
          Settings.Default.Save();
        }
      }
    }
  }
}
