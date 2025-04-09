namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class DeleteWorkspacePopup : EditorWindow
	{
		private FavoritesPersistentState favoritesState;
		private FavouritesWindow.FavouritesUndo undo;

		private void OnEnable()
		{
			minSize = new Vector2( 200, 60 );
		}

		private void OnGUI()
		{
			foreach ( var name in favoritesState.WorkspaceNames )
			{
				if ( GUILayout.Button( name ) )
				{
					Debug.LogFormat( "About to delete '{0}'", name );
					undo.CaptureStateBefore( string.Format( "Delete favorites workspace '{0}'", name ) );
					favoritesState.DeleteWorkspace( name );
					undo.CaptureStateAfter();
					EditorEx.TryGetWindow<FavouritesWindow>().Repaint();
					EditorEx.RepaintPopups();
				}
			}
		}

		public void SetDependencies( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			this.favoritesState = favoritesState;
			this.undo = undo;
		}

		public static void Show( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			var popup = EditorEx.TryGetWindow<DeleteWorkspacePopup>();

			if( popup != null )
				popup.Close();

			var window = CreateInstance<DeleteWorkspacePopup>();
			window.titleContent = new GUIContent( "Click the workspaces to delete" );
			window.SetDependencies( favoritesState, undo );
			window.ShowUtility();
			window.Focus();
		}
	}

}