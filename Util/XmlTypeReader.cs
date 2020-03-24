using System;
using System.IO;
using System.Xml.Serialization;

namespace TranslationHelper.Util {
  public static class XmlTypeReader {
    public static TResultType ReadAs<TResultType>( string filename ) {
      var type = typeof( TResultType );
      using ( FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read ) ) {
        var xs = new XmlSerializer( type );
        return (TResultType) xs.Deserialize( fs );
      }
    }

    public static void SaveTo( string filename, object content ) {
      if ( content == null ) {
        throw new ArgumentException( nameof(content) );
      }
      if ( string.IsNullOrWhiteSpace( filename ) ) {
        throw new ArgumentException( nameof( filename ));
      }
      var type = content.GetType();
      using ( var fs = new FileStream( filename, FileMode.Create, FileAccess.Write ) ) {
        var xs = new XmlSerializer(type);
        xs.Serialize( fs, content );
      }
    }
  }
}
