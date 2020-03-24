using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TranslationHelper.Converters {
  public class SimpleNameConverter : IValueConverter {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
      return Path.GetFileName( value as string );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
      throw new NotImplementedException();
    }
  }
}
