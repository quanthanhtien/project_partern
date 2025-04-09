namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class RenameWorkspacePopup : EditorWindow
	{
		private string workspaceName;
		private FavoritesPersistentState favoritesState;
		private FavouritesWindow.FavouritesUndo undo;
		private string initialName;

		public static void Show( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			RenameWorkspacePopup popup = EditorEx.TryGetWindow<RenameWorkspacePopup>();
			// NOTE(Rafa): Need to close and reopen so edited text is not preserved
			if ( popup != null )
				popup.Close();

			popup = CreateInstance<RenameWorkspacePopup>();
			popup.SetDependencies( favoritesState, undo );
			popup.initialName = favoritesState.WorkspaceNames[favoritesState.SelectedWorkspaceIdx];
			popup.workspaceName = popup.initialName;
			popup.titleContent = new GUIContent( string.Format( "Rename workspace '{0}'...", popup.initialName ) );
			popup.ShowUtility();
			popup.Focus();
		}

		private void OnEnable()
		{
			minSize = new Vector2( 300, 62 );
			maxSize = new Vector2( 10000, 62 );
		}

		private void OnGUI()
		{
			Event guiEvent = Event.current;
			if ( favoritesState == null )
				return;

			GUI.SetNextControlName( "Name" );
			workspaceName = EditorGUILayout.TextField( workspaceName );
			EditorGUI.FocusTextInControl( "Name" );

			string errorMessage = CheckErrorMessage( workspaceName );
			if ( errorMessage != string.Empty )
			{
				EditorGUILayout.HelpBox( errorMessage, MessageType.Error );
			}
			else
			{
				string buttonText = string.Format( "'{0}' to '{1}'", initialName, workspaceName );

				if ( GUILayout.Button( buttonText ) || guiEvent.isKey && guiEvent.keyCode == KeyCode.Return )
				{
					Debug.Log( buttonText );
					undo.CaptureStateBefore( string.Format( "Renamed Workspace '{0}' to '{1}'", initialName, workspaceName ) );
					favoritesState.RenameWorkspace( initialName, workspaceName );
					undo.CaptureStateAfter();
					this.Close();
					EditorEx.TryGetWindow<FavouritesWindow>().Focus();
					EditorEx.RepaintPopups();
				}
			}
		}

		private string CheckErrorMessage( string newName )
		{
			if ( Array.IndexOf( favoritesState.WorkspaceNames, newName ) > -1
				&& newName != initialName )
				return string.Format( "Cannot rename to '{0}'. Already exists", newName );
			if ( StringEx.IsNullOrWhitespace( newName ) )
				return string.Format( "Cannot rename to '{0}'. Only whitespace", newName );
			if ( Array.IndexOf( favoritesState.WorkspaceNames, initialName ) == -1 )
				return "Invalid Operation. Source workspace no longer exists";

			return string.Empty;
		}

		public void SetDependencies( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			this.favoritesState = favoritesState;
			this.undo = undo;
		}
	}

}