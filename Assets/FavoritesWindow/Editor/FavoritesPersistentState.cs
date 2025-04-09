namespace Favorites
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class FavoritesPersistentState : ScriptableObject
	{
		[SerializeField]
		private int workspaceIdx = -1;
		[SerializeField]
		private List<Workspace> workspaces = new List<Workspace>();

		private UnityGUIList<FavoriteItem> workspaceGuiList;

		private AbstractFavoriteStorage storage;
		private AbstractAssetDatabase assetDatabase;
        private AbstractSceneRefContainer sceneRefContainer;

        public string[] WorkspaceNames { get; private set; }

		// NOTE(rafa): this indirection is necessary for testing
		public static IUndoRedoEventHolder undoEvent;

		static FavoritesPersistentState()
		{
			undoEvent = new UnityUndoRedoEventHolder();
		}

		public int SelectedWorkspaceIdx
		{
			get
			{
				return workspaceIdx;
			}
		}

		public void OnBeforeSerialize()
		{

		}

		public void OnAfterDeserialize()
		{
			if ( workspaceGuiList == null )
				return;

			Debug.LogFormat( "StateAfter Perform Undo: '{0}'", DebugString() );

			int selectedIdx = workspaceIdx;
			workspaceIdx = -1;
			SetSelectedWorkspace( selectedIdx );
			this.RegenerateWorkspaceNames();
		}

		public void OnEnable()
		{
			hideFlags = HideFlags.DontSave;
			undoEvent.WhenUndoRedoPerformed -= UndoRedoPerformed;
			undoEvent.WhenUndoRedoPerformed += UndoRedoPerformed;
		}	

		public void OnDisable()
		{
			undoEvent.WhenUndoRedoPerformed -= UndoRedoPerformed;
		}

        public void DebugPrintSceneRefContainer(string name)
        {
            ((SceneRefContainer)sceneRefContainer).DebugLogKnownSceneRefs(name);
        }

		private void UndoRedoPerformed()
		{
            RegenerateViewState();
            this.Save();
        }

        private void RegenerateViewState()
        {
            if (workspaceGuiList == null)
                return;
            int selectedIdx = workspaceIdx;
            workspaceIdx = -1;
            SetSelectedWorkspace(selectedIdx);
            this.RegenerateWorkspaceNames();
        }

        public void SetSelectedWorkspace( int newIndex )
		{
			if(workspaceGuiList == null)
				return;

			if ( workspaceIdx > -1 && workspaceIdx < workspaces.Count )
				StoreWorkspaceGUIList( workspaceIdx, workspaceGuiList );
			if ( newIndex > -1 && newIndex < workspaces.Count )
				LoadWorkspaceGUIList( newIndex, workspaceGuiList );
			workspaceIdx = newIndex;
		}

        public List<Workspace> GetWorkspaces()
        {
			// NOTE: the important difference here is that this returns 
			// the current workspaces in memory, which has propagated
			// instanceIds and sceneRefGuid transient to the editor session
            return this.workspaces;
        }

        public void DeleteWorkspace( string name )
		{
			int deleteIdx = -1;
			for ( int i = 0; i < workspaces.Count; i++ )
			{
				if ( workspaces[i].name == name )
				{
					deleteIdx = i;
					break;
				}
			}

			if ( deleteIdx == -1 )
				return;

			workspaces.RemoveAt( deleteIdx );
			if ( workspaces.Count == 0 )
			{
				workspaceIdx = -1;
				EnsureNonEmpty( workspaces );
				SetSelectedWorkspace( 0 );
				RegenerateWorkspaceNames();
				return;
			}


			int selectedIdx = workspaceIdx;

			if ( deleteIdx == workspaceIdx )
				workspaceIdx = -1;
			else if ( deleteIdx < workspaceIdx )
				workspaceIdx -= 1;


			if ( deleteIdx > selectedIdx )
				SetSelectedWorkspace( selectedIdx );
			else
				SetSelectedWorkspace( selectedIdx - 1 );

			RegenerateWorkspaceNames();
		}

		public void CreateWorkspace( string name )
		{
			if ( StringEx.IsNullOrWhitespace( name ) )
				return;

			if ( !WorkspaceNames.Contains( name ) )
			{
				workspaces.Add( Workspace.Create( name ) );
				SetSelectedWorkspace( workspaces.Count - 1 );
			}
			RegenerateWorkspaceNames();
		}

		public void RenameWorkspace( string initialName, string newName )
		{
			if ( initialName == newName
				|| StringEx.IsNullOrWhitespace( newName )
				|| WorkspaceNames.Contains( newName ) )
				return;

			var workspaceIdx = Array.IndexOf( WorkspaceNames, initialName );

			if ( workspaceIdx > -1 )
			{
				workspaces[workspaceIdx].name = newName;
				RegenerateWorkspaceNames();
			}
		}

		public void Clear()
		{
			storage.ClearWorkspaces();
			workspaceIdx = -1;
		}

		public void Set( List<Workspace> workspaces )
		{
			storage.SetWorkspaces( workspaces );
		}

		public void Restore()
        {
            var storedWorkspaces = storage.GetWorkspaces();

            for (int i = 0; i < workspaces.Count; i++)
            {
                var oldWs = workspaces[i];
                for (int j = 0; j < storedWorkspaces.Count; j++)
                {
                    var newWs = storedWorkspaces[j];
                    if (newWs.name == oldWs.name)
                    {
                        newWs.instanceIds = oldWs.instanceIds;
                        newWs.sceneRefGuids = oldWs.sceneRefGuids;
                        break;
                    }
                }
            }

            EnsureNonEmpty(storedWorkspaces);

            workspaces.Clear();
            workspaces.AddRange(storedWorkspaces);

            workspaceIdx = storage.GetSelectedWorkspaceIdx();

            if (workspaces.Count == 0)
                workspaceIdx = -1;
            else if (workspaceIdx >= workspaces.Count || workspaceIdx < 0)
                workspaceIdx = 0;

            RegenerateWorkspaceNames();

            LoadSelectedWorkspace();
        }

        private void LoadSelectedWorkspace()
        {
            if (workspaceIdx > -1 && workspaceGuiList != null)
                LoadWorkspaceGUIList(workspaceIdx, workspaceGuiList);
        }

        private void RegenerateWorkspaceNames()
		{
			if ( WorkspaceNames.Length != workspaces.Count )
				WorkspaceNames = new string[workspaces.Count];

			for ( int i = 0; i < workspaces.Count; i++ )
			{
				string name = workspaces[i].name;
				WorkspaceNames[i] = name;
			}
		}

		public string DebugString()
		{
			return string.Format( "idx:{0} WS: {1}", workspaceIdx, Workspace.SerializeMany( workspaces ) );
		}

		public void Save()
		{
			this.StoreCurrentWorkspace();

			storage.SetSelectedWorkspaceIdx( workspaceIdx );
			storage.SetWorkspaces( workspaces );
		}

		public void StoreCurrentWorkspace()
		{
			if( workspaceGuiList == null)
				return;
				
			if ( workspaceIdx > -1 && workspaceGuiList != null)
				StoreWorkspaceGUIList( workspaceIdx, workspaceGuiList );
		}

		public void SetDependencies(
			AbstractFavoriteStorage storage ,
			AbstractSceneRefContainer sceneRefContainer,
			AbstractAssetDatabase assetDatabase)
		{
			this.assetDatabase = assetDatabase;
			this.WorkspaceNames = new string[] { };
			this.storage = storage;
			this.sceneRefContainer = sceneRefContainer;
		}

		public void RegisterView(UnityGUIList<FavoriteItem> guiList)
		{
			this.workspaceGuiList = guiList;
			this.LoadSelectedWorkspace();
		}

		private static void EnsureNonEmpty( List<Workspace> storedWorkspaces )
		{
			if ( storedWorkspaces.Count == 0 )
				storedWorkspaces.Add( Workspace.Create( "Default Workspace" ) );
		}

		private void StoreWorkspaceGUIList(
			int WorkspaceToStoreIdx,
			UnityGUIList<FavoriteItem> workspaceGUIList )
		{
			Workspace workspace = workspaces[WorkspaceToStoreIdx];

			var normalItems = workspaceGUIList.GetNormalItems();
			workspace.itemIds = new List<string>( normalItems.Count );
			workspace.cachedItemPaths = new List<string>( normalItems.Count );
			workspace.selectedItemIds = new List<string>( normalItems.Count );
			workspace.localIds = new List<long>(normalItems.Count);
			workspace.instanceIds = new List<int>(normalItems.Count);
			workspace.sceneRefGuids = new List<string>( normalItems.Count );
			workspace.pathCoordinates = new List<PathCoordinates>(normalItems.Count);

			for ( int i = 0; i < normalItems.Count; i++ )
			{
				var listItem = normalItems[i];
				var favorite = listItem.Value;
				workspace.itemIds.Add( favorite.guid );

				string path = listItem.Value.FullVirtualPath;
				if ( !string.IsNullOrEmpty( path ) )
					workspace.cachedItemPaths.Add( path );
				
				workspace.localIds.Add( favorite.LocalId );
				workspace.instanceIds.Add( favorite.InstanceId );
				workspace.sceneRefGuids.Add( favorite.SceneRefGuid);
				workspace.pathCoordinates.Add( new PathCoordinates()
				{
					Coords = new List<int>(favorite.PathCoordinates)
				});

				if ( listItem.IsSelected )
				{
					workspace.selectedItemIds.Add( listItem.Value.guid );
				}
			}
		}

		private void LoadWorkspaceGUIList(
			int workspaceToLoadIdx,
			UnityGUIList<FavoriteItem> workspaceGuiList )
		{
			Workspace workspace = workspaces[workspaceToLoadIdx];
			List<string> selectedItemGuids = workspace.selectedItemIds;

			PopulateGuiList( workspaceGuiList, workspace );

			var guiItems = workspaceGuiList.GetNormalItems();
			for ( int selectedGuidIdx = 0; selectedGuidIdx < selectedItemGuids.Count; ++selectedGuidIdx )
			{
				string selectedGuid = selectedItemGuids[selectedGuidIdx];

				for ( int listItemIdx = 0; listItemIdx < guiItems.Count; ++listItemIdx )
				{
					var listItem = guiItems[listItemIdx];

					if ( selectedGuid == listItem.Value.guid )
					{
						listItem.IsSelected = true;
						break;
					}
				}
			}
		}

		private void PopulateGuiList( UnityGUIList<FavoriteItem> workspaceGuiList, Workspace workspace )
		{
			workspaceGuiList.Clear();
			for ( int i = 0; i < workspace.itemIds.Count; i++ )
			{
				string itemGuid = workspace.itemIds[i];
				Debug.Log(workspace.localIds);
				long localId = i < workspace.localIds.Count ? workspace.localIds[i] : -1L;
				var pathCoords = i < workspace.pathCoordinates.Count ? workspace.pathCoordinates[i].Coords : new List<int>();
				string lastKnownPath = i < workspace.cachedItemPaths.Count ? workspace.cachedItemPaths[i] : string.Empty;
				string sceneRefGuid = i < workspace.sceneRefGuids.Count ? workspace.sceneRefGuids[i] : string.Empty;
				int instanceId = i < workspace.instanceIds.Count ? workspace.instanceIds [i] : 0;

				bool isSceneRef = 
					!string.IsNullOrEmpty(itemGuid) && 
					!string.IsNullOrEmpty(lastKnownPath) &&
					localId == -1 && pathCoords.Count > 0;

				FavoriteItem favorite = null;
				if( isSceneRef )
				{
					favorite = sceneRefContainer.Get(sceneRefGuid);
				}
				else
				{
					favorite = new FavoriteItem(  
						itemGuid, 
						lastKnownPath,
						localId,
						pathCoords,
						assetDatabase );
					favorite.InstanceId = instanceId;
				}

				workspaceGuiList.Add( favorite );
			}
		}

		public interface IUndoRedoEventHolder
		{
			event Undo.UndoRedoCallback WhenUndoRedoPerformed;
		}

		public class UnityUndoRedoEventHolder : IUndoRedoEventHolder
		{
			public event Undo.UndoRedoCallback WhenUndoRedoPerformed
			{
				add
				{
					Undo.undoRedoPerformed += value;
				}

				remove
				{
					Undo.undoRedoPerformed -= value;
				}
			}
		}
	}
}