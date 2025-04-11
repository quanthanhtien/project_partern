namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class CreateWorkspacePopup : EditorWindow
	{
		private string workspaceName;
		private FavoritesPersistentState favoritesState;
		private FavouritesWindow.FavouritesUndo undo;
		private const int InputFramesSkipTotal = 3;
		private Event EmptyEvent;
		private int skippedFrames = 0;

		public static void Show( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			CreateWorkspacePopup popup = EditorEx.TryGetWindow<CreateWorkspacePopup>();
			// NOTE(Rafa): Need to close and reopen so edited text is not preserved
			if ( popup != null )
				popup.Close();

			popup = CreateInstance<CreateWorkspacePopup>();
			popup.SetDependencies( favoritesState, undo );
			popup.titleContent = new GUIContent( "Create Workspace by name" );
			popup.workspaceName = "WorkspaceName";
			popup.ShowUtility();
			popup.Focus();

			popup.skippedFrames = 0;
		}

		private void OnEnable()
		{
			minSize = new Vector2( 200, 62 );
			maxSize = new Vector2( 10000, 62 );

			EmptyEvent = new Event() { type = EventType.Ignore };
		}

		private void OnGUI()
		{
			// NOTE(rafa): This is added to prevent the EnterKey press that opened the popup to close it inmediately
			Event guiEvent = EmptyEvent;
			if ( skippedFrames > InputFramesSkipTotal )
				guiEvent = Event.current;
			else
				skippedFrames += 1;

			GUI.SetNextControlName( "Name" );
			workspaceName = EditorGUILayout.TextField( workspaceName );
			EditorGUI.FocusTextInControl( "Name" );

			string errorMessage = CheckErrorMessage( workspaceName );
			if ( errorMessage != string.Empty )
			{
				EditorGUILayout.HelpBox( errorMessage, MessageType.Error );
			}
			else if ( GUILayout.Button( "Create Workspace" ) || guiEvent.isKey && guiEvent.keyCode == KeyCode.Return )
			{
				Debug.LogFormat( "Create workspace '{0}'", workspaceName );
				undo.CaptureStateBefore( string.Format( "Create favorites workspace '{0}'", workspaceName ) );
				this.favoritesState.CreateWorkspace( workspaceName );
				undo.CaptureStateAfter();
				this.Close();
				EditorEx.TryGetWindow<FavouritesWindow>().Focus();
				EditorEx.RepaintPopups();
			}
		}

		public void SetDependencies( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			this.favoritesState = favoritesState;
			this.undo = undo;
		}

		private string CheckErrorMessage( string name )
		{
			if ( Array.IndexOf( favoritesState.WorkspaceNames, name ) > -1 )
				return string.Format( "Invalid name '{0}'. Already exists", name );
			if ( StringEx.IsNullOrWhitespace( name ) )
				return string.Format( "Invalid name '{0}'. Only whitespace", name );

			return string.Empty;
		}
	}

}