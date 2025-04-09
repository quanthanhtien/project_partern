namespace Favorites
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class UnityIMGUI : IGUI
	{
		private readonly static GUIStyle horizontalMarker;
		private readonly static GUIContent horizontalMarkerContent;
		private float yOffset;
		private Rect? horizontalMarkerPosition;
		private Color horizontalMarkerColor;
        private Vector2 scrollPosition = Vector2.zero;

        static UnityIMGUI()
		{
			horizontalMarker = new GUIStyle();
			horizontalMarker.normal.background = EditorGUIUtility.whiteTexture;
			horizontalMarker.overflow = new RectOffset( -6, 0, -2, -2 );
			horizontalMarker.stretchWidth = true;
			horizontalMarker.fixedHeight = 6;
			horizontalMarker.margin = new RectOffset( 10, 0, 0, 0 );
			horizontalMarker.imagePosition = ImagePosition.ImageLeft;
			horizontalMarkerContent = new GUIContent( GetMarkerImage() );
		}

		// NOTE(rafa): we need these pair of methods to solve integration issues with unity imgui
		// user needs to call them at the begining at end of each onGUIcall
		public void OnGUIBegun()
		{
			// This ensures that logic that depends on get last rect does not fail on first element of a group
			CreateZeroHeightLayoutEntry();
			this.yOffset = 0;
			horizontalMarkerPosition = null;
		}

		public void OnGUIEnded()
		{
			if ( horizontalMarkerPosition != null && Event.current.type == EventType.Repaint )
			{
				var position = horizontalMarkerPosition.Value;
				var color = horizontalMarkerColor;

				Color restoreColor = GUI.color;
				GUI.color = color;
				position.y = position.y + position.height - 2 - yOffset;
				horizontalMarker.Draw( position, horizontalMarkerContent, false, false, false, false );
				GUI.color = restoreColor;
			}
		}

		public void HorizontalMarker( Color color )
		{
			var rect = GUILayoutUtility.GetLastRect();
			// NOTE(rafa): Since we draw the marker after everything
			// we need to store the position without the scroll applied
			// so it's in window coordinates
			rect.position -= scrollPosition;
			rect = BoundsCorrection(rect);
			
			this.horizontalMarkerPosition = rect;
			this.horizontalMarkerColor = color;
		}

		public void LayoutBeginVertical()
		{
			EditorGUILayout.BeginVertical();
			// This ensures that logic that depends on get last rect does not fail on first element of a group
			// with the same left margin as a label
			GUILayout.Label( GUIContent.none, GUILayout.Height( 0 ), GUILayout.ExpandWidth( true ) );
		}

		public void LayoutEndVertical()
		{
			EditorGUILayout.EndVertical();
		}

		public Rect LayoutGetLastRect()
		{
			return GUILayoutUtility.GetLastRect();
		}

		public void LayoutLabel( GUIContent content, params GUILayoutOption[] options )
		{
			GUILayout.Label( content, options );
		}

		public Vector2 LayoutBeginScrollView( Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options )
		{
			// This ensures that logic that depends on get last rect does not fail on first element of a group
			CreateZeroHeightLayoutEntry();
			Rect positionBefore = this.LayoutGetLastRect();
			this.scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, options );

			// This ensures that logic that depends on get last rect does not fail on first element of a group
			CreateZeroHeightLayoutEntry();
			Rect positionAfter = this.LayoutGetLastRect();

			// This offset is used to fix an issue from unity
			// - GUILayoutUtility.GetLastRect space changes from window to ScrollableArea
			// - after BeginScrollView is called, so any existing hit test logic will malfunction
			// - users should call BoundsCorrection to convert it back to window space
			this.yOffset = positionBefore.y - positionAfter.y;

			return this.scrollPosition;
		}

		public void LayoutEndScrollView()
		{
			this.yOffset = 0;
			EditorGUILayout.EndScrollView();
		}

		public Rect BoundsCorrection( Rect itemBounds )
		{
			itemBounds.y += this.yOffset;
			return itemBounds;
		}

		private void CreateZeroHeightLayoutEntry()
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.EndVertical();
		}

		private static Texture2D GetMarkerImage()
		{
			return EditorEx.LoadPluginIcon( "DropMarkerBadge.png" );
		}
	}

}