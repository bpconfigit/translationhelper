using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Properties;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class OpenDirectoryCommand : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var appModel = parameter as ApplicationModel;
      return appModel != null;
    }

    public override void Execute( object parameter ) {
      var appModel = parameter as ApplicationModel;
      if ( appModel == null ) {
        return;
      }
      using ( var fbd = new FolderBrowserDialog() ) {
        if ( !string.IsNullOrWhiteSpace( Settings.Default.LastActiveDirectory ) ) {
          fbd.SelectedPath = Settings.Default.LastActiveDirectory;
        }
        fbd.ShowNewFolderButton = false;
        var result = fbd.ShowDialog();
        if ( result == System.Windows.Forms.DialogResult.OK ) {
          appModel.TranslationDirectory = fbd.SelectedPath;
          Settings.Default.LastActiveDirectory = fbd.SelectedPath;
          Settings.Default.Save();
        }
      }
    }
  }
}
