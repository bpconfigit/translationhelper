using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using TranslationHelper.Annotations;
using TranslationHelper.Converters;
using TranslationHelper.Model;
using TranslationHelper.Properties;
using TranslationHelper.Util;

namespace TranslationHelper.ViewModel {
  public class CompareViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly Lazy<GridViewColumnCollection> _stringTranslationColumnCollection;
    private readonly Lazy<GridViewColumnCollection> _stringTranslationTranslationColumnCollection;

    public CompareViewModel() {
      _targetDirectory = Settings.Default.LastCompareDirectory;
      _stringTranslationColumnCollection = new Lazy<GridViewColumnCollection>( () => {
        if ( ApplicationModel == null ) {
          throw new InvalidOperationException( "Application model must be defined" );
        }
        var gvcc = new GridViewColumnCollection {
          new GridViewColumn {
            Header = "Key",
            DisplayMemberBinding = new Binding( "Item1" ),
            Width = 100
          }
        };
        var converter = new TranslatedValueConverter();
        var cultures = GetSharedCultures();
        foreach ( var culture in cultures ) {
          gvcc.Add( new GridViewColumn() {
            Header = new CultureInfo( culture ).DisplayName,
            DisplayMemberBinding = new Binding( "Item2.Values" ) { Converter = converter, ConverterParameter = culture }
          });
        }
        return gvcc;
      } );
      _stringTranslationTranslationColumnCollection = new Lazy<GridViewColumnCollection>( () => {
        if ( ApplicationModel == null ) {
          throw new InvalidOperationException( "Application model must be defined" );
        }
        var gvcc = new GridViewColumnCollection {
          new GridViewColumn {
            Header = "Key",
            DisplayMemberBinding = new Binding( "Item1" ),
            Width = 100
          }
        };
        var converter = new TranslatedValueConverter();
        var cultures = GetSharedCultures().ToList();
        foreach ( var culture in cultures ) {
          gvcc.Add( new GridViewColumn() {
            Header = $"{new CultureInfo( culture ).DisplayName} (source)",
            DisplayMemberBinding = new Binding( "Item2.Values" ) { Converter = converter, ConverterParameter = culture }
          } );
        }
        foreach ( var culture in cultures ) {
          gvcc.Add( new GridViewColumn() {
            Header = $"{new CultureInfo( culture ).DisplayName} (target)",
            DisplayMemberBinding = new Binding( "Item3.Values" ) { Converter = converter, ConverterParameter = culture }
          } );
        }
        return gvcc;
      } );
    }

    private IEnumerable<string> GetSharedCultures() {
      var sourceCultures =
        XmlTypeReader.ReadAs<AvailableCultures>(
          Path.Combine( ApplicationModel.TranslationDirectory, "AvailableCultures.xml" ) );
      var targetCultures =
        XmlTypeReader.ReadAs<AvailableCultures>(
          Path.Combine( TargetDirectory, "AvailableCultures.xml" )
        );
      return sourceCultures.Cultures.Select( c => c.Lang.ToUpper() )
        .Union( targetCultures.Cultures.Select( c => c.Lang.ToUpper() ) ).OrderBy( c => c );
    }

    private ApplicationModel _applicationModel;
    public ApplicationModel ApplicationModel {
      get => _applicationModel;
      set {
        if ( Equals( _applicationModel, value ) ) {
          return;
        }
        _applicationModel = value;
        OnPropertyChanged();
      }
    }

    public GridViewColumnCollection StringTranslationColumnCollection => _stringTranslationColumnCollection.Value;

    public GridViewColumnCollection StringTranslationTranslationColumnCollection => _stringTranslationTranslationColumnCollection.Value;

    private string _targetDirectory;
    public string TargetDirectory {
      get => _targetDirectory;
      set {
        if ( _targetDirectory == value ) {
          return;
        }
        _targetDirectory = value;
        OnPropertyChanged();
      }
    }

    private void Reset() {
      KeysWithDifferentValues.Clear();
      MissingKeys.Clear();
      ExtraKeys.Clear();
    }

    public ICollection<Tuple<string, Translation, Translation>> KeysWithDifferentValues { get; } =
      new ObservableCollection<Tuple<string, Translation, Translation>>();

    public ICollection<Tuple<string, Translation>> MissingKeys { get; } =
      new ObservableCollection<Tuple<string, Translation>>();

    public ICollection<Tuple<string, Translation>> ExtraKeys { get; } =
      new ObservableCollection<Tuple<string, Translation>>();

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public void SetValues( List<Tuple<string, Translation>> missingKeys, List<Tuple<string, Translation>> extraKeys, List<Tuple<string, Translation, Translation>> differences ) {
      Reset();
      missingKeys.ForEach( v => MissingKeys.Add(v) );
      extraKeys.ForEach( v => ExtraKeys.Add( v ) );
      differences.ForEach( v => KeysWithDifferentValues.Add( v ) );
    }
  }
}
