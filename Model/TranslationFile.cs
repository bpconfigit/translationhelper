using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace TranslationHelper.Model {
  /// <summary>
  /// Some Poco classes mixed with presentation layer, not the best option but just something I need for now
  /// </summary>
  [XmlRoot("Translations")]
  public class TranslationFile {
    private readonly Collection<Translation> _translations = new ObservableCollection<Translation>();

    [XmlElement("Translation")]
    public Collection<Translation> Translations {
      get => _translations;
      set {
        if ( Equals(_translations, value) ) {
          return;
        }
        _translations.Clear();
        if ( value == null ) {
          return;
        }
        foreach ( var val in value ) {
          _translations.Add( val );
        }
      }
    }
  }
}
