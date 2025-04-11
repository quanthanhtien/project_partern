namespace Favorites
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEngine;

	public static class ListItemDisplayPathSetter
	{
		private static List<ListItem<FavoriteItem>> itemsSortedByReverseFullPath = new List<ListItem<FavoriteItem>>(30);
		
		public static List<ListItem<FavoriteItem>> SetMinimumConflictingDisplayPaths(
			this List<ListItem<FavoriteItem>> items )
		{
			SetDisplayPaths( items );
			return items;
		}
			
		private static void SetDisplayPaths(List<ListItem<FavoriteItem>> list )
		{
			int itemsCount = list.Count;
			// Sort items by reversed full name
			itemsSortedByReverseFullPath.Clear();
			if( list.Count > itemsSortedByReverseFullPath.Capacity)
			 	itemsSortedByReverseFullPath.Capacity = list.Count;
				 
			for( int i = 0; i< itemsCount; i++)
				itemsSortedByReverseFullPath.Add(list[i]);
			
			itemsSortedByReverseFullPath.Sort(ReversePathComparer);

			int lastIdx = itemsCount -1;
			for( int i = 0; i < itemsCount; i++)
			{
				FavoriteItem currentItem = itemsSortedByReverseFullPath[i].Value;
				string current = currentItem.FullVirtualPath;

				if( i == lastIdx )
				{
					currentItem.DisplayPath = Path.GetFileName(current);
				}
				else
				{
					FavoriteItem nextItem = itemsSortedByReverseFullPath[i+1].Value;
					string next = nextItem.FullVirtualPath;
					int endingCoincidenceWithNext = CountLastCoincidentalCharacters(current,next);

					if( endingCoincidenceWithNext == 0)
					{
						currentItem.DisplayPath = Path.GetFileName(current);
					}
					else
					{
						int endingCoincidenceWithPrevious = endingCoincidenceWithNext;

						currentItem.DisplayPath =  current.Substring(GetPathCutIdx(current, endingCoincidenceWithPrevious));

						int blockSize = 2;
						int blockIdx = 0;
						do
						{
							blockIdx += 1;
							currentItem = nextItem;
							current = nextItem.FullVirtualPath;

							int nextIdx = i + blockIdx + 1;

							if( nextIdx > lastIdx)
							{
								currentItem.DisplayPath = current.Substring(GetPathCutIdx(current, endingCoincidenceWithPrevious));
								break;
							}

							nextItem = itemsSortedByReverseFullPath[nextIdx].Value;
							next = nextItem.FullVirtualPath;

							endingCoincidenceWithNext = CountLastCoincidentalCharacters(current, next);

							if( endingCoincidenceWithNext > 0 )
							{
								int endingBlockSize = 
									System.Math.Max(endingCoincidenceWithPrevious, endingCoincidenceWithNext);

								currentItem.DisplayPath = current.Substring(
									GetPathCutIdx(current, endingBlockSize));
								endingCoincidenceWithPrevious = endingCoincidenceWithNext;
								blockSize += 1;
							}
							else
							{
								currentItem.DisplayPath = current.Substring(GetPathCutIdx(current, endingCoincidenceWithPrevious));
								break;
							}
						}
						while( i + blockIdx < itemsCount );

						// This -1 is to take into consideration the outer loop increment
						i += blockSize -1; 
					}
				}
			}
		}

		private static int GetPathCutIdx(string path, int blockEndingCharCount )
		{
			int cutIdx = path.Length - blockEndingCharCount;
			if (cutIdx >= path.Length)
				cutIdx = path.Length - 1;

			char cutChar = path[cutIdx];

			bool cutIsNextToSlash = cutChar != '/' && cutIdx - 1 >= 0 && path[cutIdx -1] == '/';

			cutIdx -= cutIsNextToSlash ? 2 : 1;
			while( cutIdx > 0 )
			{
				if( path[cutIdx] != '/')
				{
					cutIdx -= 1;
				}
				else
				{
					// Note(rafa): we don't want the / at the begining of the name
					cutIdx += 1;
					break;
				}
			}
			
			return cutIdx < 0 ? 0 : cutIdx;
		}

		private static int CountLastCoincidentalCharacters(string a, string b)
		{
			int sameEndingCharCount = 0;
			int aLength = a.Length;
			int bLength = b.Length;
			int minLength = System.Math.Min(aLength, bLength);

			for( int reverseIdx = 0; reverseIdx < minLength; reverseIdx++)
			{
				int aIdx = aLength - reverseIdx - 1;
				int bIdx = bLength - reverseIdx - 1;
				if( a[aIdx] == b[bIdx])
				{
					sameEndingCharCount += 1;
				}
				else
				{
					if( reverseIdx > 0 && a[aIdx + 1] == '/' && b[bIdx + 1] == '/')
					{
						// if last characted counted is '/' we ignore it for the block
						sameEndingCharCount -= 1;
					}
					break;
				}
			}

			return sameEndingCharCount;
		}

		private static int ReversePathComparer(ListItem<FavoriteItem> x, ListItem<FavoriteItem> y)
		{
			// Negative => x is less than y
			// 0 => x equals to y
			// Positive  => x is greater than y

			string xs = x.Value.FullVirtualPath;
			string ys = y.Value.FullVirtualPath;

			int xLength = xs.Length;
			int yLength = ys.Length;

			int minLength = System.Math.Min( xLength, yLength);
			for( int i = 0; i < minLength; i++)
			{
				char xChar = xs[xLength - i - 1];
				char yChar = ys[yLength - i - 1];

				int charComparison = xChar.CompareTo(yChar);
				if( charComparison != 0)
				{
					return charComparison;
				}
			}

			return 0;

			// What do we want 
			//  x/c   -> x/a
			//  x/b   -> x/b
			//  x/a   -> x/c
		}

		private static int CountMinimimSegments(List<ListItem<FavoriteItem>> itemsWithSameBaseName )
		{

			int itemsCount = itemsWithSameBaseName.Count;
			int minLength = int.MaxValue;
			int segmentCount = 0;

			for( int i = 0; i < itemsCount; i++)
			{
				minLength = Mathf.Min(minLength, itemsWithSameBaseName[i].Value.FullVirtualPath.Length);
			}

			int breakCharIdx = minLength - 1;

			for( int i = 0; i < minLength; i++ )
			{
				bool allDifferent = true;

				// We start on second because first will be always equals to itself
				for( int pivotIdx = 0; pivotIdx < itemsCount; pivotIdx++)
				{
					var pivot = itemsWithSameBaseName[pivotIdx];
					char pivotChar = pivot.Value.FullVirtualPath[pivot.Value.FullVirtualPath.Length -1 - i];
					
					for ( int candidateIdx = 0; candidateIdx < itemsCount; candidateIdx++)
					{
						if( candidateIdx == pivotIdx)
							continue;

						var candidate = itemsWithSameBaseName[candidateIdx];
						char candidateChar = candidate.Value.FullVirtualPath[candidate.Value.FullVirtualPath.Length - 1 - i];
						allDifferent &= ( pivotChar != candidateChar );
					}
				}

				if( allDifferent )
				{
					breakCharIdx = i;
					break;
				}

			}


			for( int i = 0; i< breakCharIdx + 1; i++ )
			{
				char c = itemsWithSameBaseName[0].Value.FullVirtualPath[i];
				if( c == '/')
					segmentCount += 1;
			}

			return segmentCount;
		}

		private static bool EndWithSameName(string a, string b)
		{
			int aLength = a.Length;
			int bLength = b.Length;
			int minLength = Mathf.Min(a.Length, b.Length);

			char nameTerminal = '/';
			bool result = true;

			for( int i = 0; i < minLength; i++ )
			{
				char aChar = a[aLength - 1 - i];
				char bChar = b[bLength - 1 - i];

				if( aChar != bChar)
				{
					result = false;
					break;
				}
				else if (aChar == nameTerminal && bChar == nameTerminal)
				{
					break;
				}
			}

			return result;
		}

		private static string LastPathSegments( string text, int segmentCount )
		{
			var segments = text.Split( '/' );
			int skipCount = segments.Length - segmentCount;
			return string.Join( "/", segments.Skip( skipCount ).ToArray() );
		}
	}

}