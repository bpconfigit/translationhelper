using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TranslationHelper.ViewModel;

namespace TranslationHelper.Converters {
  public class CultureToLangConverter : IValueConverter {
    public TranslationResources TranslationModel { get; set; }
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
      if ( TranslationModel == null ) {
        throw new InvalidOperationException( $"{nameof( TranslationModel )} is not defined in the converter" );
      }
      if ( TranslationModel.AvailableCultures == null ) {
        throw new InvalidOperationException( $"{nameof( TranslationModel.AvailableCultures )} were not loaded yet" );
      }
      var suggestedCulture = value as string;
      return TranslationModel.AvailableCultures.Cultures.FirstOrDefault(
        cultureInfo => string.Equals(cultureInfo.Lang, suggestedCulture, StringComparison.CurrentCultureIgnoreCase) || cultureInfo.Lang.StartsWith( $"{suggestedCulture}-", StringComparison.CurrentCultureIgnoreCase ) );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
      var cultureInfo = value as string;
      if ( string.IsNullOrWhiteSpace( cultureInfo ) ) {
        return value;
      }
      return cultureInfo.Split( '-' )[0];
    }
  }
}
