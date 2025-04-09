namespace Favorites
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public interface IGUI
	{
		void LayoutBeginVertical();
		void LayoutLabel( GUIContent content, params GUILayoutOption[] options );
		void HorizontalMarker( Color color );
		Rect LayoutGetLastRect();
		void LayoutEndVertical();
		Vector2 LayoutBeginScrollView( Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options );
		void LayoutEndScrollView();
		// Note(Rafa): this method is necessary to workaround unity imgui issues
		// - EditorGUILayout.BeginScrollView changes the space of GUILayoutUtility.GetLastRect to be local to the scroll view
		// - breaking hit test logic
		Rect BoundsCorrection( Rect itemBounds );
	}

}