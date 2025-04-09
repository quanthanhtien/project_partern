namespace Favorites
{
	using System;
	using UnityEditor;
	using UnityEngine;

	public class ListEventDiscriminator
	{
		private bool isMousePressed;
		private int itemHitOnMouseDown_Idx = -1;
		private bool isDragging;
		private double lastTapTime = 0;
		private int lastTapIdx = -2;
		private IClock clock;

		public ListEventDiscriminator( IClock clock )
		{
			this.clock = clock;
		}

		public event Action<int> WhenTapped;
		public event Action<int> WhenDragged;
		public event Action WhenDragUpdated;
		public event DropAction WhenDropped;
		public event Action<int> WhenDoubleTapped;

		public delegate void DropAction( int dropIdx );

		public void MouseDown_OnItem( int itemIdx )
		{
			Debug.LogFormat( "Down({0})", itemIdx );
			isMousePressed = true;
			itemHitOnMouseDown_Idx = itemIdx;
		}

		public void MouseDown_OnEmptySpace()
		{
			Debug.LogFormat( "Down({0})", -1 );
			isMousePressed = true;
		}

		public void MouseUp_OnItem( int itemIdx )
		{
			Debug.LogFormat( "Up({0})", itemIdx );
			if ( !isMousePressed )
				return;

			if ( itemHitOnMouseDown_Idx > -1 && !isDragging )
				OnElementWasTapped( itemHitOnMouseDown_Idx );
			isMousePressed = false;
			itemHitOnMouseDown_Idx = -1;
		}

		public void MouseUp_OnEmptySpace()
		{
			Debug.LogFormat( "Up({0})", -1 );
			if ( !isMousePressed )
				return;

			if ( !isDragging )
				OnElementWasTapped( -1 );
			isMousePressed = false;
		}

		public void MouseDragged_OverItem( int itemIdx )
		{
			Debug.LogFormat( "Drag({0})", itemIdx );
			if ( isMousePressed && itemHitOnMouseDown_Idx == itemIdx && !isDragging )
			{
				itemHitOnMouseDown_Idx = itemIdx;
				OnDragStarted( itemHitOnMouseDown_Idx );
				isDragging = true;
			}
		}

		public void MouseDragged_OnEmptySpace()
		{
			Debug.LogFormat( "Drag({0})", -1 );
		}

		public void MouseDropped_OverItem( int itemIdx )
		{
			Debug.LogFormat( "Drop({0})", itemIdx );
			this.OnElementsWereDropped( itemIdx );
		}

		public void MouseDropped_OnEmptySpace( Space space )
		{
			int dropIdx = space == Space.AfterList ? -1 : 0;
			Debug.LogFormat( "Drop({0})", dropIdx );
			this.OnElementsWereDropped( dropIdx );
		}

		public void MouseDrag_UpdatedOnPanel()
		{
			Debug.Log( "Mouse dragged over panel" );
			this.OnDragUpdated();
		}

		public void LoseFocus()
		{
			Debug.Log( "Lost Focus" );
			isMousePressed = false;
			isDragging = false;
		}

		private void OnElementWasTapped( int itemTappedIdx )
		{
			bool isDoubleTap = false;

			if ( itemTappedIdx != lastTapIdx )
			{
				lastTapIdx = itemTappedIdx;
				lastTapTime = clock.GetNowInSeconds();
			}
			else
			{
				double tapTime = clock.GetNowInSeconds();
				double tapDeltaTime = tapTime - lastTapTime;

				isDoubleTap = tapDeltaTime < 0.5f;
				if( isDoubleTap )
				{
					lastTapTime = 0;
					lastTapIdx = -2; // -1 represents tap on empty space
				}
				else
				{
					lastTapTime = tapTime;
					lastTapIdx = itemTappedIdx;
				}
			}

			if ( isDoubleTap )
			{
				if ( WhenDoubleTapped != null )
					WhenDoubleTapped( itemTappedIdx );
			}
			else if ( WhenTapped != null )
				WhenTapped( itemTappedIdx );


		}

		private void OnDragStarted( int itemHitOnMouseDown_Idx )
		{
			if ( WhenDragged != null )
				WhenDragged( itemHitOnMouseDown_Idx );
		}

		private void OnDragUpdated()
		{
			if ( WhenDragUpdated != null )
				WhenDragUpdated();
		}

		private void OnElementsWereDropped( int destIdx )
		{
			if ( WhenDropped != null )
				WhenDropped( destIdx );
			isDragging = false;
			itemHitOnMouseDown_Idx = -1;
		}

		public enum Space
		{
			AfterList,
			BeforeList
		}
	}

}