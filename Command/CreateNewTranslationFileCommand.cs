using System.IO;
using System.Windows.Forms;
using TranslationHelper.Model;
using TranslationHelper.Util;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class CreateNewTranslationFile : AbstractCommandManagerCommand {
    public override bool CanExecute( object parameter ) {
      var translationModel = parameter as TranslationResources;
      return !string.IsNullOrWhiteSpace( translationModel?.ApplicationModel?.TranslationDirectory );
    }

    public override void Execute( object parameter ) {
      var translationModel = parameter as TranslationResources;

      var defaultDirectory = Path.Combine( translationModel.ApplicationModel.TranslationDirectory, "Overrides" );
      if ( !Directory.Exists( defaultDirectory ) ) {
        Directory.CreateDirectory( defaultDirectory );
      }
      using ( var sfd = new SaveFileDialog() ) {
        
        sfd.InitialDirectory = Path.Combine( translationModel.ApplicationModel.TranslationDirectory, "Overrides" );
        var result = sfd.ShowDialog();
        if ( result == DialogResult.OK ) {
          var filename = sfd.FileName;
          XmlTypeReader.SaveTo( filename, new TranslationFile() );
          translationModel.TranslationFiles.Add( filename );
        }
      }
    }
  }
}
