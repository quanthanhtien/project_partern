namespace Favorites
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public delegate T DroppedElementBuilder<T>( UnityEngine.Object instance, string path );

	// NOTE(rafa): this is a required helper class to substitute for Event
	// the reason is that Event.type setter does not work in batch mode so some test fail when
	// running in batch mode because the setted type is not preserved
	public class GUIEvent
	{

		public EventType type { get; set; }
		public Vector2 mousePosition { get; set; }
		public bool isMouse { get; set; }
		public bool isSelectAll { get; set; }
		public bool isUpArrow { get; set; }
		public bool isDownArrow { get; set; }
		public bool isShift { get; set; }
		public bool isControl { get; set; }
		public bool isCommand { get; set; }

		public GUIEvent() { }
		public GUIEvent( Event current )
		{
			current.AcceptSelectAll();
			isSelectAll = current.IsSelectAll();
			isUpArrow = current.isKey && current.keyCode == KeyCode.UpArrow && current.type == EventType.KeyDown;
			isDownArrow = current.isKey && current.keyCode == KeyCode.DownArrow && current.type == EventType.KeyDown;
			isShift = current.shift;
			isControl = current.control;
			isCommand = current.command;

			if (isUpArrow || isDownArrow)
				current.Use();

			type = current.type;
			mousePosition = current.mousePosition;
			isMouse = current.isMouse;
		}
	}

	public class UnityGUIList<T> where T : IDraggable
	{
		private Func<ListItem<T>, Rect> drawListItem;
		private Action drawEmptyCase;
		private List<ListItem<T>> listItems = new List<ListItem<T>>();
		private List<ListItem<T>> visibleItems = new List<ListItem<T>>();
		private List<ListItem<T>> transientItems = new List<ListItem<T>>();
		private List<ListItem<T>> itemsMarkedForRemoval = new List<ListItem<T>>();
		private ListEventDiscriminator listEventDiscriminator = new ListEventDiscriminator( new EditorClock() );
		private ListEventsHandler<T> eventsHandler;
		private IGUI gui;
		private Color horizontalMarkerColor = new Color( 51f / 255, 87f / 255, 217f / 255 );
		private Vector2 scrollPosition;

		private float maxPanelHeight = 10000;

		public UnityGUIList(
			Func<ListItem<T>, Rect> drawFavorite,
			Action drawEmptyCase,
			DroppedElementBuilder<T> elementBuilder,
			IGUI gui,
			IDragAndDrop dragAndDrop,
			ListEventDiscriminator listEventDiscriminator,
			Vector2 initialScrollPosition ):
			this(drawFavorite, drawEmptyCase, elementBuilder,gui,dragAndDrop, listEventDiscriminator )
		{
			this.scrollPosition = initialScrollPosition;
		}
		public UnityGUIList(
			Func<ListItem<T>, Rect> drawFavorite,
			Action drawEmptyCase,
			DroppedElementBuilder<T> elementBuilder,
			IGUI gui,
			IDragAndDrop dragAndDrop,
			ListEventDiscriminator listEventDiscriminator )
		{
			drawListItem = drawFavorite;
			this.drawEmptyCase = drawEmptyCase;
			eventsHandler = new ListEventsHandler<T>( listItems, transientItems, elementBuilder, dragAndDrop );
			this.gui = gui;
			this.listEventDiscriminator = listEventDiscriminator;
			this.listEventDiscriminator.WhenTapped += EventDiscriminator_OnTapped;
			this.listEventDiscriminator.WhenTapped += _ => listWasModified = true;
			this.listEventDiscriminator.WhenDragged += EventDiscriminator_OnDragStarted;
			this.listEventDiscriminator.WhenDropped += eventsHandler.OnItemsWereDroppedInside;
			this.listEventDiscriminator.WhenDropped += _ => listWasModified = true;
			this.listEventDiscriminator.WhenDragUpdated += eventsHandler.OnDragUpdated;
		}

		public UnityGUIList(
			Func<ListItem<T>, Rect> drawFavorite,
			Action drawEmptyCase,
			DroppedElementBuilder<T> elementBuilder,
			IGUI gui,
			IDragAndDrop dragAndDrop ) :
			this( drawFavorite, drawEmptyCase, elementBuilder, gui, dragAndDrop, new ListEventDiscriminator( new EditorClock() ) )
		{
		}

		public Vector2 ScrollPosition
		{
			get { return this.scrollPosition; }
		}

		public List<ListItem<T>> GetNormalItems()
		{
			return listItems;
		}

		public List<ListItem<T>> GetVisibleItems()
		{
			return visibleItems;
		}

		private void EventDiscriminator_OnDragStarted( int itemIdx )
		{
			eventsHandler.OnDragStarted( itemIdx, GetModifierKey() );
		}

		private void EventDiscriminator_OnTapped( int tappedIdx )
		{
			eventsHandler.OnElementWasTapped( tappedIdx, GetModifierKey() );
		}

		private static ModifierKey GetModifierKey()
		{
			Event e = Event.current;
			bool isControlModifier = 
				Application.platform == RuntimePlatform.OSXEditor ?
				e.command : e.control;

			ModifierKey modifier = ModifierKey.None;
			if ( isControlModifier )
				modifier = ModifierKey.Control;
			else if ( e.shift )
				modifier = ModifierKey.Shift;
			return modifier;
		}

		int nextDropMarkerIdx = -1;
		bool showDropMarker = false;
		List<ListItem<T>> itemsToDraw = new List<ListItem<T>>();
		private bool listWasModified;

		public bool DoList( GUIEvent e, float maxPanelHeight)
		{
			this.maxPanelHeight = maxPanelHeight;
			return DoList(e);
		}

		public int GetMaxListItemsInWindow(float maxPanelHeight, float itemHeight)
		{
			return Mathf.CeilToInt(maxPanelHeight / itemHeight);
		}

		public bool DoList( GUIEvent e )
		{
			listWasModified = false;
			bool isAdditiveSelection = e.isShift
				|| (Application.platform == RuntimePlatform.OSXEditor ? e.isCommand : e.isControl);

			if ( e.isSelectAll )
				eventsHandler.OnSelectAll();
			else if ( e.isUpArrow )
			{
				if ( isAdditiveSelection )
					eventsHandler.OnShiftUpArrow();
				else
					eventsHandler.OnUpArrow();
			}
			else if ( e.isDownArrow )
			{
				if ( isAdditiveSelection )
					eventsHandler.OnShiftDownArrow();
				else
					eventsHandler.OnDownArrow();
			}

			Vector2 mousePositionInScroll = e.mousePosition + scrollPosition;

			if ( e.type == EventType.DragUpdated )
				listEventDiscriminator.MouseDrag_UpdatedOnPanel();

			if ( e.type == EventType.DragUpdated )
				showDropMarker = true;
			else if ( e.type == EventType.DragExited )
				this.OnLostFocus();
			else if ( e.type == EventType.Ignore || e.type == EventType.DragPerform )
                showDropMarker = false;

			scrollPosition = gui.LayoutBeginScrollView( scrollPosition, false, false );
			gui.LayoutBeginVertical();

			// used to determine if the event should be routed to empty space
			bool anyItemHovered = false;

			// Copy the items to draw so layout changes do not happend during draw
			itemsToDraw.Clear();
			itemsToDraw.AddRange( eventsHandler.ListToDraw );
			var items = eventsHandler.ListToDraw;

			// Used so layout does not change during the loop
			int currentDropMarkerIdx = nextDropMarkerIdx;

			float itemHeight = 20;
			int totalItems = itemsToDraw.Count;
			int maxInWindow = GetMaxListItemsInWindow (maxPanelHeight , itemHeight );
			int firstInWindow = Mathf.FloorToInt( scrollPosition.y / itemHeight );
			firstInWindow = Mathf.Clamp(firstInWindow, 0, Mathf.Max(0, totalItems - maxInWindow ));
			int lastInWindow = Mathf.Min( firstInWindow + maxInWindow, totalItems ) - 1;

			float invisibleItemsHeightBefore = firstInWindow * itemHeight;
			gui.LayoutLabel(GUIContent.none, GUILayout.Height(invisibleItemsHeightBefore));

			visibleItems.Clear();
			for ( int i = firstInWindow; i <= lastInWindow; i++ )
			{
				// Actually Draw the drop marker above list items, or in between items
				if ( currentDropMarkerIdx == i && showDropMarker )
				{
					// Color picked from unity editor drop marker
					gui.HorizontalMarker( horizontalMarkerColor );
				}

				var item = itemsToDraw[i];
				this.visibleItems.Add(item);
				// Actually Draw the list items
				Rect itemBounds = drawListItem( item );
				itemBounds = gui.BoundsCorrection( itemBounds );

				// Expand bounds so it doesn't miss the selection in boundaries
				itemBounds.y -= 1;
				itemBounds.height += 2;
				bool isHoveringItem = itemBounds.Contains( mousePositionInScroll ) && IsMouseOrDrag( e );

				// Used to determine that no item will receive the event
				anyItemHovered |= isHoveringItem;

				// For insertion point we check the hover of the upper space in between items
				itemBounds.y -= itemBounds.height / 2;
				bool isHoveringInsertionPoint = itemBounds.Contains( mousePositionInScroll ) && IsMouseOrDrag( e );

				// Save the drop marker idx for the next draw
				// Necessary to do it this way to avoid layout errors with imgui
				if ( isHoveringInsertionPoint )
				{
					nextDropMarkerIdx = i;

					if ( e.type == EventType.DragPerform )
						listEventDiscriminator.MouseDropped_OverItem( i );
				}

				// Route events to the items
				if ( isHoveringItem )
				{
					if ( e.type == EventType.MouseDown )
						listEventDiscriminator.MouseDown_OnItem( i );
					else if ( e.type == EventType.MouseUp )
						listEventDiscriminator.MouseUp_OnItem( i );
					else if ( e.type == EventType.DragUpdated || e.type == EventType.MouseDrag )
						listEventDiscriminator.MouseDragged_OverItem( i );
				}

			}

			if ( itemsToDraw.Count == 0 )
				drawEmptyCase();

			// Actualy Draw the dropmarker ( empty space below list case )
			if ( currentDropMarkerIdx == -1 && showDropMarker )
			{
				gui.HorizontalMarker( horizontalMarkerColor );
			}

			float invisibleItemsHeightAfter = (totalItems - lastInWindow - 1) * itemHeight;
			// Add a little space at the end to ease drop when scrolling
			invisibleItemsHeightAfter += 5;
			gui.LayoutLabel(GUIContent.none, GUILayout.Height(invisibleItemsHeightAfter));

			// Determine whether empty space event is above first or below last
			Rect lastItemRect = gui.LayoutGetLastRect();

			if ( !anyItemHovered && IsMouseOrDrag( e ) )
			{
				if ( mousePositionInScroll.y > lastItemRect.yMax )
					nextDropMarkerIdx = -1;
				else
					nextDropMarkerIdx = 0;
			}

			gui.LayoutEndVertical();
			gui.LayoutEndScrollView();

			RemoveItemsMarkedForRemoval();

			// Events on empty space
			if ( !anyItemHovered && IsMouseOrDrag( e ) )
			{
				if ( e.type == EventType.MouseDown )
					listEventDiscriminator.MouseDown_OnEmptySpace();
				else if ( e.type == EventType.MouseUp )
					listEventDiscriminator.MouseUp_OnEmptySpace();
				else if ( e.type == EventType.DragUpdated || e.type == EventType.MouseDrag )
					listEventDiscriminator.MouseDragged_OnEmptySpace();
				else if ( e.type == EventType.DragPerform )
					listEventDiscriminator.MouseDropped_OnEmptySpace( nextDropMarkerIdx == -1 ? ListEventDiscriminator.Space.AfterList : ListEventDiscriminator.Space.BeforeList );
			}

			return listWasModified;
		}

		private static bool IsMouseOrDrag( GUIEvent e )
		{
			return (
							e.isMouse
							|| e.type == EventType.DragUpdated
							|| e.type == EventType.DragPerform );
		}

		private void RemoveItemsMarkedForRemoval()
		{
			for ( int i = 0; i < itemsMarkedForRemoval.Count; i++ )
			{
				var item = itemsMarkedForRemoval[i];
				listItems.Remove( item );
			}
			itemsMarkedForRemoval.Clear();
		}

		public void OnLostFocus()
		{
			listEventDiscriminator.LoseFocus();
			eventsHandler.List_LostFocus();
			showDropMarker = false;
		}

		public void Clear()
		{
			listItems.Clear();
		}

		public void AddRange( IEnumerable<T> values )
		{
			listItems.AddRange( values.Select( CreateListItemFromValue ) );
		}

		public void Add( T value )
		{
			listItems.Add( CreateListItemFromValue( value ) );
		}

		private ListItem<T> CreateListItemFromValue( T value )
		{
			return new ListItem<T>()
			{
				Value = value,
				IsSelected = false
			};
		}

		public void Remove( T value )
		{
			var listItem = listItems.Find( li => li.Value.Equals( value ) );
			if ( listItem != null )
				itemsMarkedForRemoval.Add( listItem );
		}
	}

	public interface IDraggable
	{
		UnityEngine.Object Asset { get; }
		string AssetPath { get; }
	}
}