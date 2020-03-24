using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TranslationHelper.Annotations;

namespace TranslationHelper.Model {
  public class Translation : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly Collection<Value> _values = new ObservableCollection<Value>();
    private string _key;

    [XmlAttribute]
    public string Key {
      get => _key;
      set {
        if ( _key == value ) {
          return;
        }
        _key = value;
        OnPropertyChanged();
      }
    }

    [XmlElement("Value")]
    public Collection<Value> Values {
      get => _values;
      set {
        if ( Equals( _values, value ) ) {
          return;
        }
        _values.Clear();
        if ( value == null ) {
          return;
        }
        foreach ( var val in value ) {
          _values.Add( val );
        }
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}