using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WindowPlugins.GUITVSeries.Extensions
{
    static class StringExtensions
    {
        public static string ToSlug( this string aText )
        {
            string lValue = aText.Normalize( NormalizationForm.FormD ).Trim();
            StringBuilder lBuilder = new StringBuilder();

            foreach ( char c in aText.ToCharArray() )
            {
                if ( CharUnicodeInfo.GetUnicodeCategory( c ) != UnicodeCategory.NonSpacingMark )
                {
                    lBuilder.Append( c );
                }
            }
            lValue = lBuilder.ToString();

            byte[] lBytes = Encoding.GetEncoding( "Cyrillic" ).GetBytes( aText );

            lValue = Regex.Replace( Regex.Replace( Encoding.ASCII.GetString( lBytes ), @"\s{2,}|[^\w]", " ", RegexOptions.ECMAScript ).Trim(), @"\s+", "-" );

            return lValue.ToLowerInvariant();
        }
    }
}
