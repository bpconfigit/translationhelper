using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TranslationHelper.Annotations;

namespace TranslationHelper.ViewModel {
  public class ApplicationModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private string _translationDirectory;
    private string _selectedFile;

    public string TranslationDirectory {
      get => _translationDirectory; 
      set {
        if ( _translationDirectory == value ) {
          return;
        }
        _translationDirectory = value;
        OnPropertyChanged();
      }
    }

    public string SelectedFile {
      get => _selectedFile;
      set {
        if ( _selectedFile == value ) {
          return;
        }
        _selectedFile = value;
        OnPropertyChanged();
      }
    }

    public Window SearchDialog { get; set; }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}
