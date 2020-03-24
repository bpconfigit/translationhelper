using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TranslationHelper.Annotations;
using TranslationHelper.Command;
using TranslationHelper.Model;
using TranslationHelper.Util;

namespace TranslationHelper.ViewModel {
  public class SearchModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    public ApplicationModel ApplicationModel { get; set; }
    public TranslationResources TranslationResources { get; set; }

    public IList<Tuple<string, Translation>> Results { get; } = new ObservableCollection<Tuple<string, Translation>>();

    private bool _didSearch;
    private bool _includeTranslations = true;
    private bool _includeKeys = true;
    private string _searchValue;

    public ICommand HideWindowCommand { get; }

    public ICommand SearchCommand { get; }

    public ICommand NavigateToViewCommand { get; }

    public SearchModel() {
      HideWindowCommand = new DelegateCommand( o => true, o => ( (Window)o ).Hide() );
      SearchCommand = new DelegateCommand( o => !string.IsNullOrWhiteSpace( SearchValue ) && !DidSearch,
        o => Search() );
      NavigateToViewCommand = new DelegateCommand( 
        tuple => (tuple as Tuple<string, Translation>) != null,
        tuple => NavigateToTranslation( tuple as Tuple<string, Translation> ) );
    }

    public bool DidSearch {
      get => _didSearch;
      set {
        if ( _didSearch == value ) {
          return;
        }
        _didSearch = value;
        OnPropertyChanged();
      }
    }

    public string SearchValue {
      get => _searchValue;
      set {
        if ( _searchValue == value ) {
          return;
        }
        _searchValue = value;
        OnPropertyChanged();
        DidSearch = false;
      }
    }

    public bool IncludeTranslations {
      get => _includeTranslations;
      set {
        if ( _includeTranslations == value ) {
          return;
        }
        _includeTranslations = value;
        OnPropertyChanged();
        DidSearch = false;
      }
    }

    public bool IncludeKeys {
      get => _includeKeys;
      set {
        if ( _includeKeys == value ) {
          return;
        }
        _includeKeys = value;
        OnPropertyChanged();
        DidSearch = false;
      }
    }

    private void NavigateToTranslation( Tuple<string, Translation> item ) {
      if ( item == null ) {
        return;
      }
      // select the file
      ApplicationModel.SelectedFile = item.Item1;
      // this will cause the correct file to be loaded and then we have to set the active translation still
      var translation = item.Item2;
      TranslationResources.CurrentSelected =
        TranslationResources.CurrentTranslation.Translations.FirstOrDefault( t => t.Key == translation.Key );
    }

    private void Search() {
      DidSearch = true;
      Results.Clear();
      var path = ApplicationModel.TranslationDirectory;
      var files = Directory.GetFiles( path, "*.xml", SearchOption.AllDirectories );
      foreach ( string file in files ) {
        if ( file.IndexOf( "AvailableCultures", StringComparison.CurrentCultureIgnoreCase ) >= 0 ) {
          continue;
        }
        var filename = Path.Combine( path, file );
        var translationContent = XmlTypeReader.ReadAs<TranslationFile>( Path.Combine( path, file ) );
        var matches = GetMatchesFor( translationContent.Translations ).ToList();
        if ( matches.Count == 0 ) {
          continue;
        }
        foreach ( var match in matches ) {
          Results.Add( new Tuple<string, Translation>( filename, match ) );
        }
      }
    }

    private IEnumerable<Translation> GetMatchesFor( Collection<Translation> translationContentTranslations ) {
      foreach ( var item in translationContentTranslations ) {
        if ( IncludeKeys && item.Key?.IndexOf( SearchValue, StringComparison.CurrentCultureIgnoreCase ) >= 0 ) {
          yield return item;
          continue;
        }
        if ( IncludeTranslations &&
             item.Values.Any(
               val => val.Content?.IndexOf( SearchValue, StringComparison.CurrentCultureIgnoreCase ) >= 0 ) ) {
          yield return item;
        }
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}
