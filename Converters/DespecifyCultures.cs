using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TranslationHelper.Model;

namespace TranslationHelper.Converters {
  public class DespecifyCultures : IValueConverter {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
      var cultures = value as AvailableCultures;
      if ( cultures == null ) {
        return value;
      }
      var results = new List<string>();
      foreach ( var c in cultures.Cultures) {
        results.Add( c.Lang.ToLower() );
        results.Add( c.Lang.Split('-')[0].ToLower() );
      }
      return results;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
      throw new NotImplementedException();
    }
  }
}
