namespace Favorites
{
	using System.Collections;
	using System.Collections.Generic;
	using Favorites;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	[System.Serializable]
	public class FavoriteWindowEditorStorage : AbstractFavoriteStorage
	{
		public string WorkspacesListKey { get; private set; }
		public string WorkspaceIdxKey { get; private set; }

		public FavoriteWindowEditorStorage(
			string preferencesKey )
		{
			this.WorkspacesListKey = preferencesKey + "_WorkspacesList";
			this.WorkspaceIdxKey = preferencesKey + "_SelectedWorkspaceIdx";
		}

		public override void ClearWorkspaces()
		{
			EditorPrefs.DeleteKey( this.WorkspacesListKey );
		}

		public override List<Workspace> GetWorkspaces()
		{
			var workspaces = Workspace.GetAll( EditorPrefs.GetString( this.WorkspacesListKey ) );

			return workspaces;
		}

		public override void SetWorkspaces( List<Workspace> workspaces )
		{
			EditorPrefs.SetString( this.WorkspacesListKey, Workspace.SerializeMany( workspaces ) );
		}

		public override int GetSelectedWorkspaceIdx()
		{
			return EditorPrefs.GetInt( WorkspaceIdxKey, -1 );
		}

		public override void SetSelectedWorkspaceIdx( int newIndex )
		{
			EditorPrefs.SetInt( WorkspaceIdxKey, newIndex );
		}
	}

}