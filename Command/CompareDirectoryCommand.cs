using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TranslationHelper.Model;
using TranslationHelper.Util;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Command {
  public class CompareDirectoryCommand : AbstractCommandManagerCommand {
    private bool _isActive;

    public bool IsActive {
      get => _isActive;
      set {
        if ( _isActive == value ) {
          return;
        }
        _isActive = value;
        CommandManager.InvalidateRequerySuggested();
      }
    }

    public override bool CanExecute( object parameter ) {
      // has to have the directories set, that are different from eachother and they have to exist
      var model = parameter as CompareViewModel;
      return !IsActive && model?.ApplicationModel?.TranslationDirectory != null &&
             model.TargetDirectory != null && Directory.Exists( model.ApplicationModel.TranslationDirectory ) &&
             Directory.Exists( model.TargetDirectory ) && model.ApplicationModel.TranslationDirectory != model.TargetDirectory;
    }

    private TranslationFile GetTranslations( string filename ) {
      if (filename.Contains("AvailableCultures")) {
        return null;
      }
      return XmlTypeReader.ReadAs<TranslationFile>( filename );
    }

    private IDictionary<string, Translation> ReadDirectory( string directory ) {
      var result = new Dictionary<string, Translation>( StringComparer.CurrentCultureIgnoreCase );
      var files = Directory.GetFiles( directory, "*.xml", SearchOption.AllDirectories );
      foreach ( var file in files ) {
        var tf = GetTranslations( file );
        if ( tf == null ) {
          continue;
        }
        foreach ( var translation in tf.Translations ) {
          if ( result.ContainsKey( translation.Key ) ) {
            if ( !file.Contains( "/Overrides/" ) ) {
              // could throw error here but okay
              continue;
            }
            result.Remove( translation.Key );
          }
          result.Add( translation.Key, translation );
        }
      }
      return result;
    }

    public override void Execute( object parameter ) {
      var model = parameter as CompareViewModel;
      if ( model == null ) {
        return;
      }
      _isActive = true;
      var dispatcher = Dispatcher.CurrentDispatcher;

      Task.Run( () => {
        var sourceDictionary = ReadDirectory( model.ApplicationModel.TranslationDirectory );
        var targetDictionary = ReadDirectory( model.TargetDirectory );

        var missingKeys = new List<Tuple<string, Translation>>();
        var extraKeys = new List<Tuple<string, Translation>>();
        var differences = new List<Tuple<string, Translation, Translation>>();

        foreach ( var key in sourceDictionary.Keys ) {
          var sf = sourceDictionary[key];
          if ( !targetDictionary.TryGetValue( key, out var tf ) ) {
            missingKeys.Add( new Tuple<string, Translation>( key, sf ) );
            continue;
          }
          var langSourceDict = sf.Values.GroupBy( v => v.Lang )
            .ToDictionary( v => v.Key, v => string.Join( Environment.NewLine, v ) );
          var langTargetDict = tf.Values.GroupBy( v => v.Lang )
            .ToDictionary( v => v.Key, v => string.Join( Environment.NewLine, v ) );
          foreach ( var lang in langSourceDict ) {
            if ( !langTargetDict.ContainsKey( lang.Key ) ) {
              differences.Add( new Tuple<string, Translation, Translation>( key, sf, tf ) );
              break;
            }
            if ( !string.Equals( langTargetDict[lang.Key], lang.Value, StringComparison.CurrentCultureIgnoreCase ) ) {
              differences.Add( new Tuple<string, Translation, Translation>( key, sf, tf ) );
            }
          }
        }
        foreach ( var key in targetDictionary.Keys ) {
          if ( !sourceDictionary.ContainsKey( key ) ) {
            extraKeys.Add( new Tuple<string, Translation>( key, targetDictionary[key] ) );
          }
        }
        dispatcher.Invoke(() => {
          model.SetValues( missingKeys, extraKeys, differences );
        });
        IsActive = false;
      } );
    }
  }
}
