using System;
using System.IO;

namespace Favorites
{
	public static class StringEx
	{
		public static bool IsNullOrWhitespace( string value )
		{
			if ( value != null )
			{
				for ( int i = 0; i < value.Length; i++ )
				{
					if ( !char.IsWhiteSpace( value[i] ) )
					{
						return false;
					}
				}
			}

			return true;
		}

		public static string ShortenAssetPath( this string assetPath )
		{
			string result = assetPath;
			string assetsBegining = "Assets/";
			if ( assetPath.StartsWith( assetsBegining ) )
				result = result.Substring( assetsBegining.Length );
			string directory = Path.GetDirectoryName( result );
			if ( !string.IsNullOrEmpty( directory ) )
				result = Path.GetFileName( result ) + " @ " + directory;

			return result;
		}
	}
}