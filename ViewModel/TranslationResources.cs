using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using TranslationHelper.Annotations;
using TranslationHelper.Model;
using TranslationHelper.Util;

namespace TranslationHelper.ViewModel {
  public class TranslationResources : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _changed;
    private ObjectTracker _activeCollectionTracker;
    private ApplicationModel _applicationModel;
    private TranslationFile _currentTranslation;
    private Translation _currentSelected;
    private AvailableCultures _availableCultures;

    public ApplicationModel ApplicationModel {
      get => _applicationModel;
      set {
        if ( value == _applicationModel ) {
          return;
        }
        if ( _applicationModel != null ) {
          _applicationModel.PropertyChanged -= OnApplicationModelChanged;
        }
        _applicationModel = value;
        if ( _applicationModel != null ) {
          _applicationModel.PropertyChanged += OnApplicationModelChanged;
          LoadAvailableFiles();
          LoadCurrentSelectedFile();
        }
      }
    }

    public AvailableCultures AvailableCultures {
      get => _availableCultures;
      set {
        if ( Equals( _availableCultures, value ) ) {
          return;
        }
        _availableCultures = value;
        OnPropertyChanged();
      }
    }

    public bool Changed {
      get => _changed;
      set {
        if ( _changed == value ) {
          return;
        }
        _changed = value;
        OnPropertyChanged();
      }
    }

    public TranslationFile CurrentTranslation {
      get => _currentTranslation;
      set {
        if ( Equals( value, _currentTranslation ) ) {
          return;
        }
        if ( _currentTranslation != null ) {
          _activeCollectionTracker?.Dispose();
          // unhook potential changes here
        }
        _currentTranslation = value;
        OnPropertyChanged();
        if ( _currentTranslation != null ) {
          // we probably need to observe changes within this somehow
          _activeCollectionTracker = new ObjectTracker( _currentTranslation, OnTrackedObjectChanged );
        }
        Changed = false;
      }
    }

    public Translation CurrentSelected {
      get => _currentSelected;
      set {
        if ( Equals( _currentSelected, value ) ) {
          return;
        }
        _currentSelected = value;
        OnPropertyChanged();
      }
    }

    private void OnApplicationModelChanged( object sender, PropertyChangedEventArgs e ) {
      if ( e.PropertyName == nameof(ApplicationModel.TranslationDirectory) ) {
        LoadAvailableFiles();
      }
      if ( e.PropertyName == nameof(ApplicationModel.SelectedFile) ) {
        LoadCurrentSelectedFile();
      }
    }

    private void LoadCurrentSelectedFile() {
      if ( string.IsNullOrWhiteSpace( ApplicationModel.SelectedFile ) ) {
        CurrentTranslation = null;
        return;
      }
      CurrentTranslation = XmlTypeReader.ReadAs<TranslationFile>( ApplicationModel.SelectedFile );
    }

    private void LoadAvailableFiles() {
      // clear any previous selection
      ApplicationModel.SelectedFile = null;
      // need to reload the data
      TranslationFiles.Clear();
      var directory = ApplicationModel.TranslationDirectory;
      if ( string.IsNullOrWhiteSpace( directory ) ) {
        return;
      }
      if ( !Directory.Exists( directory ) ) {
        return;
      }
      var files = Directory.GetFiles( directory, "*.xml", SearchOption.AllDirectories );
      foreach ( var file in files ) {
        if ( file.IndexOf( "AvailableCultures", StringComparison.CurrentCultureIgnoreCase ) >= 0 ) {
          AvailableCultures = XmlTypeReader.ReadAs<AvailableCultures>( Path.Combine( directory, file ) );
          continue;
        }
        TranslationFiles.Add( Path.Combine( directory, file ) );
      }
    }

    public void SaveActive() {
      if ( CurrentTranslation == null ) {
        return;
      }
      if ( string.IsNullOrWhiteSpace( ApplicationModel.SelectedFile ) ) {
        return;
      }
      XmlTypeReader.SaveTo( ApplicationModel.SelectedFile, CurrentTranslation );
      Changed = false;
    }

    public void RevertChanges() {
      LoadCurrentSelectedFile();
    }

    public void DeleteSelected() {
      CurrentTranslation.Translations.Remove( CurrentSelected );
      CurrentSelected = null;
    }

    public void CreateNewTranslation() {
      var translation = new Translation();
      CurrentTranslation.Translations.Add( translation );
      CurrentSelected = translation;
    }

    private void OnTrackedObjectChanged() {
      Changed = true;
    }

    public IList<string> TranslationFiles { get; } = new ObservableCollection<string>();

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}
