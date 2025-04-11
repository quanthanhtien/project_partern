namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[System.Serializable]
	public abstract class AbstractFavoriteStorage
	{
		public abstract List<Workspace> GetWorkspaces();
		public abstract void SetWorkspaces( List<Workspace> workspaces );
		public abstract void ClearWorkspaces();
		public abstract int GetSelectedWorkspaceIdx();
		public abstract void SetSelectedWorkspaceIdx( int newIndex );
	}

}
