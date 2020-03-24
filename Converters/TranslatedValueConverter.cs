using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TranslationHelper.Model;

namespace TranslationHelper.Converters {
  public class TranslatedValueConverter : IValueConverter {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
      var key = parameter as string ?? "en";
      if ( value is IEnumerable<Value> values ) {
        var valueList = values.ToList();
        var foundStartsWith = valueList.FirstOrDefault( v => v.Lang.StartsWith( key, StringComparison.CurrentCultureIgnoreCase ) )?.Content;
        if ( !string.IsNullOrWhiteSpace( foundStartsWith ) ) {
          return foundStartsWith;
        }
        if ( key.Contains( "-" ) ) {
          var simpleKey = key.Split( '-' )[0];
          return valueList.FirstOrDefault( v => v.Lang.StartsWith( simpleKey, StringComparison.CurrentCultureIgnoreCase ) )?.Content;
        }
      }
      return null;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
      throw new NotImplementedException();
    }
  }
}
