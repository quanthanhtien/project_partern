namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class ListEventsHandler<T> where T : IDraggable
	{
		private bool drawTransient;
		private DroppedElementBuilder<T> droppedValueBuilder;
		private MultiSelect<T> normalMultiSelection;
		private MultiSelect<T> transientMultiSelection;
		private List<ListItem<T>> normalItems = new List<ListItem<T>>();
		private List<ListItem<T>> transientItems = new List<ListItem<T>>();
		private IDragAndDrop dragAndDrop;
		private ISelector selector;
		private List<UnityEngine.Object> objectInstanceSelection = new List<UnityEngine.Object>();
		private AbstractAssetDatabase assetDatabase;

		public List<ListItem<T>> ListToDraw
		{
			get
			{
				return drawTransient ? transientItems : normalItems;
			}
		}

		public ListEventsHandler(
			List<ListItem<T>> listItems,
			List<ListItem<T>> transientItems,
			DroppedElementBuilder<T> droppedValueBuilder,
			IDragAndDrop dragAndDrop = null,
			ISelector selector = null ,
			AbstractAssetDatabase assetDatabase = null)
		{
			normalItems = listItems;
			this.transientItems = transientItems;
			normalMultiSelection = new MultiSelect<T>( normalItems );
			transientMultiSelection = new MultiSelect<T>( this.transientItems );
			this.droppedValueBuilder = droppedValueBuilder;
			this.selector = selector ?? new SelectorUnity();
			this.assetDatabase = assetDatabase ?? new UnityAssetDatabase ();
			this.dragAndDrop = dragAndDrop ?? new DragAndDropUnity();
		}

		public void OnElementWasTapped( int tappedItemIdx, ModifierKey modifierKey )
		{
			OnItemTapped( tappedItemIdx, modifierKey, normalMultiSelection );
		}

		public void OnDragUpdated()
		{
			dragAndDrop.VisualMode = DragAndDropVisualMode.Link;
		}

		public void OnDragStarted( int dragTargetIdx, ModifierKey modifier = ModifierKey.None )
		{
			Debug.LogFormat( "Started Drag on element {0}", dragTargetIdx );

			EnsureTransientSameSizeAsNormal();
			if ( dragTargetIdx == -1 )
				normalMultiSelection.ClearSelection();

			for ( int i = 0; i < normalItems.Count; i++ )
			{
				var transient = transientItems[i];
				var normal = normalItems[i];
				transient.IsSelected = normal.IsSelected;
				transient.Value = normal.Value;
			}

			if ( dragTargetIdx == -1 )
			{
				return;
			}

			drawTransient = true;
			OnItemTapped( dragTargetIdx, modifier, transientMultiSelection );
			StartDrag();
		}

		private void EnsureTransientSameSizeAsNormal()
		{
			int sizeDifferece = normalItems.Count - transientItems.Count;
			if ( sizeDifferece > 0 )
			{
				for ( int i = 0; i < sizeDifferece; i++ )
					transientItems.Add( new ListItem<T>() );
			}
			else if ( sizeDifferece < 0 )
			{
				transientItems.RemoveRange( transientItems.Count + sizeDifferece - 1, -sizeDifferece );
			}
		}

		public void OnItemsWereDroppedInside( int dropIdx )
		{
			Debug.LogFormat( "Dropped items on index '{0}'", dropIdx.ToString() );
			drawTransient = false;

			dragAndDrop.AcceptDrag();


			// Note(rafa): load references just in time to workaround crash in mac
			bool hasReferencesAssigned = dragAndDrop != null && dragAndDrop.Objects.Length > 0;
			var selectedObjects = dragAndDrop.Objects;
			string[] paths = dragAndDrop.Paths;
			var draggables = dragAndDrop.Draggables;

			// Note(luis): prevents drags from outside the window retain information from previous drags
			dragAndDrop.Draggables = null;

			int pathCount = paths.Length;
			if (!hasReferencesAssigned && pathCount > 0)
			{
				selectedObjects = new UnityEngine.Object[pathCount];
				for (int i = 0; i < pathCount; i++)
					selectedObjects [i] = assetDatabase.LoadAssetAtPath<UnityEngine.Object> (paths [i]);
			}
			else
			{	
				// NOTE(rafa): path may be empty or unsynched so we dont trust it
				int refCount = selectedObjects.Length;
				paths = new string[selectedObjects.Length];
				for( int i = 0; i < refCount; i++)
					paths[i] = assetDatabase.GetAssetPath(selectedObjects[i]);
			}

			if(draggables == null || draggables.Length != selectedObjects.Length)
			{
				draggables = new IDraggable[selectedObjects.Length]; 
			}
			
			List<ListItem<T>> selectedListItems = new List<ListItem<T>>();
			for ( int i = 0; i < selectedObjects.Length; i++ )
			{
				var value = selectedObjects[i];
				var path = paths[i];
				var draggable = draggables[i];

				if( draggable == null )
				{
					// may still happen for references from unsaved scenes
					// that are no longer on disk ( new file, or deleted scene that is loaded)
					if (value == null || string.IsNullOrEmpty(path)
					|| (path.EndsWith(".unity") && assetDatabase.Exists(path) == false))
						continue;
				}
				else
				{
					if (path.EndsWith(".unity") && assetDatabase.Exists(path) == false)
						continue;
				}

				// Find list item if any
				int listItemIdx = -1;
				for ( int j = 0; j < normalItems.Count; j++ )
				{
					var item = normalItems[j];

					if(draggable != null)
					{
						if (ReferenceEquals(item.Value, draggable))
						{
							listItemIdx = j;
							break;
						}
					}
					else
					{
						if ( value == item.Value.Asset )
						{
							listItemIdx = j;
							break;
						}
					}
				}

				var listItem = listItemIdx != -1 ? normalItems[listItemIdx] : null;
				if ( listItem != null )
				{
					if ( listItemIdx < dropIdx )
						dropIdx -= 1;

					normalItems.Remove( listItem );
					selectedListItems.Add( listItem );
				}
				else if( value != null )
				{
					listItem = new ListItem<T>()
					{
						Value = droppedValueBuilder( value, path )
					};
					selectedListItems.Add( listItem );
				}
			}

			normalMultiSelection.ClearSelection();
			for ( int i = 0; i < selectedListItems.Count; i++ )
			{
				var item = selectedListItems[i];
				item.IsSelected = true;
				normalItems.Add( item );
			}

			MoveSelectionBeforeDestIdx( dropIdx );
			int firstSelectedIdx = normalItems.FindIndex( li => li.IsSelected );
			if ( firstSelectedIdx > -1 )
			{
				// This block fixes the issue of broken multiselection after moving several selected items within the list
				normalMultiSelection.ClearSelection();
				normalMultiSelection.SimpleSelect( firstSelectedIdx );
				normalMultiSelection.ShiftSelect( firstSelectedIdx + selectedListItems.Count - 1 );
			}
		}

		public void OnSelectAll()
		{
			normalMultiSelection.SelectAll();
		}

		// -1 means none of the items are part of favorites list
		private int GetDragBeginIdxInNormalItems( UnityEngine.Object[] selectedItems )
		{
			int dragIdx = -1;

			for ( int i = 0; i < normalItems.Count && dragIdx == -1; i++ )
			{
				var normalItem = normalItems[i];
				for ( int selectedIdx = 0; selectedIdx < selectedItems.Length; selectedIdx++ )
				{
					if ( ReferenceEquals( normalItem.Value.Asset, selectedItems[selectedIdx] ) )
					{
						dragIdx = i;
						break;
					}
				}
			}

			return dragIdx;
		}

		public void List_LostFocus()
		{
			drawTransient = false;
		}

		private void StartDrag()
		{
			List<string> paths = new List<string>();
			List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
			List<IDraggable> draggedItems = new List<IDraggable>();

			for ( int i = 0; i < transientItems.Count; i++ )
			{
				ListItem<T> item = transientItems[i];
				if ( item.IsSelected )
				{
					objects.Add( item.Value.Asset );
					paths.Add( item.Value.AssetPath );
					draggedItems.Add(item.Value);
				}
			}

			dragAndDrop.PrepareStartDrag();
			dragAndDrop.VisualMode = DragAndDropVisualMode.Link;
			dragAndDrop.Objects = objects.ToArray();
			dragAndDrop.Paths = paths.ToArray();
			dragAndDrop.Draggables = draggedItems.ToArray();
			dragAndDrop.StartDrag( "Dragging favourite" );
		}

		private void OnItemTapped( int tappedItemIdx, ModifierKey modifierKey, MultiSelect<T> multiselection )
		{
			Debug.LogFormat( "Element {0} was tapped", tappedItemIdx );
			if ( tappedItemIdx == -1 )
			{
				multiselection.ClearSelection();
				return;
			}

			if ( multiselection == transientMultiSelection && modifierKey == ModifierKey.None
				&& transientItems[tappedItemIdx].IsSelected )
			{
				return; // Simple dragging from a selected element should not change selection
			}

			if ( modifierKey == ModifierKey.Control )
				multiselection.ControlSelect( tappedItemIdx );
			else if ( modifierKey == ModifierKey.Shift )
				multiselection.ShiftSelect( tappedItemIdx );
			else
			{
				multiselection.SimpleSelect( tappedItemIdx );
			}

			SetEditorSeletion( multiselection );
		}

		private void SetEditorSeletion( MultiSelect<T> multiselection )
		{
			if ( multiselection == normalMultiSelection )
			{
				objectInstanceSelection.Clear();
				foreach ( var item in normalItems )
				{
					if ( item.IsSelected )
					{
						if(item.Value.Asset != null)
							objectInstanceSelection.Add( item.Value.Asset );
						else
							objectInstanceSelection.Add( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item.Value.AssetPath));
					}
				}
				selector.SetSelection( objectInstanceSelection.ToArray() );
			}
		}

		private void Add( T value )
		{
			normalItems.Add( new ListItem<T>()
			{
				Value = value,
				IsSelected = false
			} );
		}

		private void MoveSelectionBeforeDestIdx( int endIdx )
		{
			List<ListItem<T>> selectedElements = ExtractSelectedItems( ref endIdx );
			if ( endIdx > -1 )
				normalItems.InsertRange( endIdx, selectedElements );
			else
				normalItems.AddRange( selectedElements );
		}

		private List<ListItem<T>> ExtractSelectedItems( ref int destIdx )
		{
			var result = new List<ListItem<T>>();

			int initDestIdx = destIdx;
			int leftShift = 0;
			int selectionSize = 0;

			for ( int i = 0; i < normalItems.Count; i++ )
			{
				if ( normalItems[i].IsSelected )
				{
					if ( i < destIdx )
						leftShift += 1;

					selectionSize += 1;
					result.Add( normalItems[i] );
				}
			}

			for ( int i = 0; i < result.Count; i++ )
			{
				normalItems.Remove( result[i] );
			}

			int correctedDestIdx = initDestIdx - leftShift;
			if ( correctedDestIdx >= normalItems.Count )
				correctedDestIdx = -1;

			destIdx = correctedDestIdx;
			return result;
		}

		public void OnUpArrow()
		{
			normalMultiSelection.MoveUp();
		}

		public void OnDownArrow()
		{
			normalMultiSelection.MoveDown();
		}

		public void OnShiftDownArrow()
		{
			normalMultiSelection.ShiftMoveDown();
		}

		public void OnShiftUpArrow()
		{
			normalMultiSelection.ShiftMoveUp();
		}
	}

	public enum ModifierKey
	{
		None,
		Control,
		Shift
	}

	public class DragAndDropUnity : IDragAndDrop
	{
		private string [] NoPaths = new string[]{};
		private UnityEngine.Object [] NoReferences = new UnityEngine.Object[]{};

		public DragAndDropUnity()
		{
		}

		public void AcceptDrag()
		{
			DragAndDrop.AcceptDrag();
		}

		public string[] Paths
		{
			get
			{
				return DragAndDrop.paths;
			}

			set
			{
				DragAndDrop.paths = value ?? NoPaths;
			}
		}
			
		public UnityEngine.Object[] Objects
		{
			get
			{
				return DragAndDrop.objectReferences;
			}

			set
			{
				DragAndDrop.objectReferences = value ?? NoReferences;
			}
		}

		public DragAndDropVisualMode VisualMode
		{
			get
			{
				return DragAndDrop.visualMode;
			}

			set
			{
				DragAndDrop.visualMode = value;
			}
		}

		public IDraggable[] Draggables { get; set; }

		public void PrepareStartDrag()
		{
			DragAndDrop.PrepareStartDrag();
		}

		public void StartDrag( string title )
		{
			DragAndDrop.StartDrag( title );
		}
	}

	public class SelectorUnity : ISelector
	{
		public void SetSelection( UnityEngine.Object[] objects )
		{
			var paths = objects.Select( o => AssetDatabase.GetAssetOrScenePath( o ) ).ToArray();
			if ( Array.TrueForAll( paths, path => Directory.Exists( path ) ) )
			{
				EditorEx.ShowFolderContents( objects );
			}
			else
			{
				Selection.objects = objects;
				if ( objects.Length == 1 )
					EditorGUIUtility.PingObject( objects[0] );
			}
		}
	}

}