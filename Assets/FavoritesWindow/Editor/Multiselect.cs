namespace Favorites
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;

	public class MultiSelect<T>
	{
		private List<ListItem<T>> listItems;
		private int rangeTopIdx;
		private int rangeBottomIdx;
		private int pivotIdx;

		public bool IsEmpty
		{
			get
			{
				return listItems.TrueForAll( item => !item.IsSelected );
			}
		}

		public MultiSelect( List<ListItem<T>> listItems )
		{
			this.listItems = listItems;
			ClearPivotAndTerminals();
		}

		public void ShiftSelect( int selectedIdx )
		{
			ModifiedSelect( selectedIdx, Mode.Shift );
		}

		public void ControlSelect( int selectedIdx )
		{
			ModifiedSelect( selectedIdx, Mode.Control );
		}

		public void SimpleSelect( int selectedIdx )
		{
			if ( IsIdxOutOfRange( selectedIdx ) )
				return;

			for ( int i = 0; i < listItems.Count; i++ )
			{
				listItems[i].IsSelected = ( selectedIdx == i );
			}

			if ( listItems[selectedIdx].IsSelected )
				pivotIdx = selectedIdx;
		}

		public void ClearSelection()
		{
			foreach ( var item in listItems )
			{
				item.IsSelected = false;
			}
		}

		public void SelectAll()
		{
			foreach ( var item in listItems )
			{
				item.IsSelected = true;
			}
		}

		private void ClearPivotAndTerminals()
		{
			rangeBottomIdx = -1;
			rangeTopIdx = -1;
			pivotIdx = -1;
		}

		private void ModifiedSelect( int selectedIdx, Mode mode )
		{
			if ( IsIdxOutOfRange( selectedIdx ) )
				return;

			var item = listItems[selectedIdx];
			var selectedItems = listItems
				.Select( ( it, idx ) => new { it, idx } )
				.Where( o => o.it.IsSelected );
			int selectedCount = selectedItems.Count();
			if ( selectedCount == 0 )
			{
				item.IsSelected = true;
				pivotIdx = selectedIdx;
			}
			else if ( selectedCount == 1 )
			{
				var other = selectedItems.First();
				rangeTopIdx = Math.Min( selectedIdx, other.idx );
				rangeBottomIdx = Math.Max( selectedIdx, other.idx );

				if ( mode == Mode.Shift )
				{
					SelectWholeRange();
				}
				else
				{
					item.IsSelected = !item.IsSelected;

					if ( selectedIdx == other.idx )
						pivotIdx = -1;
				}
			}
			else
			{
				bool withinRange = selectedIdx >= rangeTopIdx && selectedIdx <= rangeBottomIdx;
				bool isTerminal = selectedIdx == rangeTopIdx || selectedIdx == rangeBottomIdx;
				bool isPivot = selectedIdx == pivotIdx;

				if ( mode == Mode.Shift )
				{
					if ( withinRange )
					{
						if ( isTerminal )
						{
							if ( isPivot )
								SimpleSelect( selectedIdx );
						}
						else
						{
							if ( pivotIdx == rangeTopIdx )
								rangeBottomIdx = selectedIdx;
							else
								rangeTopIdx = selectedIdx;
							SelectWholeRange();
						}
					}
					else
					{
						if ( pivotIdx == rangeTopIdx )
						{
							if ( selectedIdx < rangeTopIdx )
							{
								rangeTopIdx = selectedIdx;
								pivotIdx = rangeBottomIdx;
							}
							else if ( selectedIdx > rangeTopIdx )
							{
								rangeBottomIdx = selectedIdx;
							}
						}
						else // Pivot is range bottom
						{
							if ( selectedIdx > rangeBottomIdx )
							{
								rangeBottomIdx = selectedIdx;
								pivotIdx = rangeTopIdx;
							}
							else if ( selectedIdx < rangeBottomIdx )
							{
								rangeTopIdx = selectedIdx;
							}
						}
						SelectWholeRange();
					}

				}
				else if ( mode == Mode.Control )
				{
					bool pivotWasTop = rangeTopIdx == pivotIdx;
					if ( selectedIdx <= rangeTopIdx )
					{
						rangeTopIdx = listItems.FindIndex( li => li.IsSelected );
						if ( pivotWasTop )
							pivotIdx = rangeBottomIdx;
					}
					else if ( selectedIdx >= rangeBottomIdx )
					{
						rangeBottomIdx = listItems.FindLastIndex( li => li.IsSelected );
						if ( !pivotWasTop )
							pivotIdx = rangeTopIdx;
					}
					item.IsSelected = !item.IsSelected;
				}
			}
		}

		private bool IsIdxOutOfRange( int index )
		{
			return index < 0 || index >= listItems.Count;
		}

		private void SelectWholeRange()
		{
			for ( int i = 0; i < listItems.Count; i++ )
			{
				listItems[i].IsSelected = ( i <= rangeBottomIdx && i >= rangeTopIdx );
			}
		}

		private enum Mode
		{
			Control,
			Shift
		}

		public void MoveDown()
		{
			int selectedCount = listItems.Count( i => i.IsSelected );

			if ( selectedCount == 0 )
				SimpleSelect( 0 );
			else if ( selectedCount == 1 )
			{
				SimpleSelect( pivotIdx + 1 );
			}
			else
			{
				int movingIdx = pivotIdx == rangeTopIdx ? rangeBottomIdx : rangeTopIdx;
				SimpleSelect( movingIdx + 1 );
			}
		}

		public void MoveUp()
		{
			int selectedCount = listItems.Count( i => i.IsSelected );

			if ( selectedCount == 0 )
				this.SimpleSelect( 0 );
			else if ( selectedCount == 1 )
			{
				SimpleSelect( pivotIdx - 1 );
			}
			else
			{
				int movingIdx = pivotIdx == rangeTopIdx ? rangeBottomIdx : rangeTopIdx;
				SimpleSelect( movingIdx - 1 );
			}
		}

		public void ShiftMoveDown()
		{
			int selectedCount = listItems.Count( i => i.IsSelected );

			if ( selectedCount == 0 )
				this.ShiftSelect( 0 );
			else if ( selectedCount == 1 )
			{
				this.ShiftSelect( pivotIdx + 1 );
			}
			else
			{
				int movingIdx = pivotIdx == rangeTopIdx ? rangeBottomIdx : rangeTopIdx;
				this.ShiftSelect( movingIdx + 1 );
			}
		}

		public void ShiftMoveUp()
		{
			int selectedCount = listItems.Count( i => i.IsSelected );

			if ( selectedCount == 0 )
				this.ShiftSelect( 0 );
			else if ( selectedCount == 1 )
				this.ShiftSelect( pivotIdx - 1 );
			else
			{
				int movingIdx = pivotIdx == rangeTopIdx ? rangeBottomIdx : rangeTopIdx;
				this.ShiftSelect( movingIdx - 1 );
			}
		}
	}
}