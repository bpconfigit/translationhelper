using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TranslationHelper.Annotations;

namespace TranslationHelper.Model {
  public class Value : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private string _lang;
    private string _content;

    [XmlAttribute]
    public string Lang {
      get => _lang;
      set {
        if ( _lang == value ) {
          return;
        }
        _lang = value?.ToLower();
        OnPropertyChanged();
      }
    }

    [XmlText]
    public string Content {
      get => _content;
      set {
        if ( _content == value ) {
          return;
        }
        _content = value;
        OnPropertyChanged();
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}