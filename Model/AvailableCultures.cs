using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TranslationHelper.Annotations;

namespace TranslationHelper.Model {
  [XmlRoot("AvailableCultures")]
  public class AvailableCultures {
    private readonly Collection<CultureDescription> _cultures = new ObservableCollection<CultureDescription>();

    [XmlElement( "Culture" )]
    public Collection<CultureDescription> Cultures {
      get => _cultures;
      set {
        if ( _cultures == value ) {
          return;
        }
        _cultures.Clear();
        if ( value != null ) {
          foreach ( var val in value ) {
            _cultures.Add( val );
          }
        }
      }
    }
  }

  public class CultureDescription : INotifyPropertyChanged {
    private string _lang;

    [XmlAttribute]
    public string Lang {
      get => _lang;
      set {
        if ( _lang == value ) {
          return;
        }
        _lang = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}
