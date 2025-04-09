namespace Favorites
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using System.Text;

    public abstract class AbstractSceneRefContainer : ScriptableObject
    {
        public abstract FavoriteItem Get(string sceneRefGuid);
        public abstract FavoriteItem GetNew(UnityEngine.Object reference, string assetPath);
        public abstract void Clear();
    }

    public class SceneRefContainer : AbstractSceneRefContainer, 
      ISerializationCallbackReceiver
    {
		private Dictionary<string, FavoriteItem> sceneRefsByGuid = new Dictionary<string, FavoriteItem>();

        [SerializeField]
        private List<KeyValuePair> dictSerialized = new List<KeyValuePair>();

        private AbstractAssetDatabase assetDatabase;


		private bool projectDidUpdate = true;
		private bool hierarchyDidUpdate = true;

        private FavoritesPersistentState favoritesPersistentState;

        public void OnEnable()
		{
			hideFlags = HideFlags.DontSave;
		}

        public void SetDependendcies(
            FavoritesPersistentState favoritesPersistentState,
            AbstractAssetDatabase assetDatabase)
        {
            this.assetDatabase = assetDatabase;
            this.favoritesPersistentState = favoritesPersistentState;

            // Initialize dictionary from the serialized kvps
            sceneRefsByGuid.Clear();
            foreach( var kvp in dictSerialized)
            {
                sceneRefsByGuid.Add(kvp.key, new FavoriteItem(
                    kvp.favorite.assetGuid,
                    kvp.favorite.cachedPath,
                    kvp.favorite.localId,
                    kvp.favorite.pathCoords,
                    assetDatabase)
                    {
                        InstanceId = kvp.favorite.instanceID,
                        SceneRefGuid = kvp.favorite.sceneRefGuid
                    });
            }

            EnsureAllExistingSceneRefsAreTracked();

            EnsureItemsAreInitialized();

            #pragma warning disable 618
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            EditorApplication.projectWindowChanged += OnProjectChanged;
            #pragma warning restore 618
            EditorApplication.update += OnEditorUpdated;
        }

        private void EnsureItemsAreInitialized()
        {
            foreach (var kvp in sceneRefsByGuid)
            {
                kvp.Value.EnsureInitialized();
            }
        }

        private void EnsureAllExistingSceneRefsAreTracked()
        {
            var workspaces = favoritesPersistentState.GetWorkspaces();

            for( int workspaceIdx = 0; workspaceIdx < workspaces.Count; workspaceIdx++)
            {
                var ws = workspaces[workspaceIdx];
                for( int i = 0; i< ws.itemIds.Count; i++)
                {
                    string guid = ws.itemIds[i];
                    string cachedPath = i < ws.cachedItemPaths.Count ? ws.cachedItemPaths[i] : string.Empty;
                    long localId = ws.localIds[i];
                    List<int> pathCoords = ws.pathCoordinates[i].Coords;
                    int instanceId = ws.instanceIds[i];
                    string sceneRefGuid = ws.sceneRefGuids[i];
                    
                    if(pathCoords.Count > 0 && string.IsNullOrEmpty(sceneRefGuid))
                    {
                        FavoriteItem newFav = new FavoriteItem(
                            guid, cachedPath, 
                            localId, pathCoords,
                            assetDatabase);

                        if(newFav.Asset != null)
                            newFav.InstanceId = newFav.Asset.GetInstanceID();
                        
                        string newSceneRefGuid = Guid.NewGuid().ToString();
                        newFav.SceneRefGuid = newSceneRefGuid;
                        // This is needed so scene refs of unloaded
                        // workspaces can be propagated correctly
                        ws.sceneRefGuids[i] = newSceneRefGuid;
                        sceneRefsByGuid.Add(newSceneRefGuid, newFav);
                    } 
                }
            }
        }

        public override void Clear()
        {
            sceneRefsByGuid.Clear();
            dictSerialized.Clear();
            var workspaces = favoritesPersistentState.GetWorkspaces();
            for( int i = 0; i < workspaces.Count; i++)
            {
                var ws = workspaces[i];
                for( int itemIdx = 0; itemIdx < ws.itemIds.Count; itemIdx++)
                {
                    ws.sceneRefGuids[itemIdx] = string.Empty;
                }
            }
        }

        public void OnDisable()
        {
            #pragma warning disable 618
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
            EditorApplication.projectWindowChanged -= OnProjectChanged;
            #pragma warning restore 618
            EditorApplication.update -= OnEditorUpdated;
        }

        public void OnBeforeSerialize()
        {
            dictSerialized.Clear();
            foreach( var kvp in sceneRefsByGuid)
            {
                dictSerialized.Add( new KeyValuePair()
                {
                    key = kvp.Key,
                    favorite = new Favorite()
                    {
                        assetGuid = kvp.Value.guid,
                        cachedPath = kvp.Value.FullVirtualPath,
                        localId = kvp.Value.LocalId,
                        pathCoords = kvp.Value.PathCoordinates,
                        instanceID = kvp.Value.InstanceId,
                        sceneRefGuid = kvp.Value.SceneRefGuid
                    }
                });
            }
        }

        public void OnAfterDeserialize()
        {
        }

        public override FavoriteItem Get( string sceneRefGuid )
        {
            var result = GetInternal( sceneRefGuid ); 
            if( result == null)
                return null;

            if(result.ShouldReset(projectDidUpdate, hierarchyDidUpdate))
                result.Reset();

            return result;
        }

        private FavoriteItem GetInternal(string sceneRefGuid)
        {
            FavoriteItem favoriteItem;
            if (sceneRefsByGuid.TryGetValue(sceneRefGuid, out favoriteItem))
            {
                return favoriteItem;
            }
            else

            return null;
        }

        public void DebugLogKnownSceneRefs(string name)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}  ", name);
            foreach (var sceneref in sceneRefsByGuid)
            {
                var fi = sceneref.Value;
                sb.AppendFormat("{0}({1})",
                    fi.FullVirtualPath,
                    string.Join(",", fi.PathCoordinates.Select(c => c.ToString()).ToArray()));
            }
            UnityEngine.Debug.Log(sb.ToString());
        }

        public override FavoriteItem GetNew(
            UnityEngine.Object objectReference, 
            string objectPath)
        {
            string dontDestroyScene = "DontDestroyOnLoad";
            string itemGuid = objectPath != dontDestroyScene ? assetDatabase.AssetPathToGUID(objectPath) : dontDestroyScene;
            string lastKnownPath = assetDatabase.GetSceneRefFullPath(objectReference);
            List<int> pathCoords = assetDatabase.GetSceneRefCoords(objectReference);
            long localId = -1L;

            FavoriteItem newFav = new FavoriteItem(
                itemGuid, lastKnownPath, 
                localId, pathCoords,
                assetDatabase);

            newFav.InstanceId = newFav.Asset.GetInstanceID();
            
            string newSceneRefGuid = Guid.NewGuid().ToString();
            newFav.SceneRefGuid = newSceneRefGuid;
            sceneRefsByGuid.Add(newSceneRefGuid, newFav);

            return newFav;
        }

        private void OnProjectChanged()
		{
			this.projectDidUpdate = true;
		}

		private void OnHierarchyChanged()
		{
			this.hierarchyDidUpdate = true;
		}

        private void OnEditorUpdated()
        {
            if(assetDatabase == null)
                return;
            int sceneCount = assetDatabase.GetLoadedScenesCount();
            for(int i=0; i<sceneCount; i++)
            {
                Scene scene = assetDatabase.GetLoadedSceneAt(i);
                string guid = assetDatabase.AssetPathToGUID(scene.path);

                foreach( var kvp in sceneRefsByGuid)
                {
                    FavoriteItem item = kvp.Value;
                    if(guid == item.guid && 
                       item.ShouldReset(projectDidUpdate, hierarchyDidUpdate))
                    {
                        item.Reset();
                    }
                }
            }

            EnsureItemsAreInitialized();
            this.projectDidUpdate = false;
            this.hierarchyDidUpdate = false;
        }

        [System.Serializable]
        public class KeyValuePair
        {
            public string key;
            public Favorite favorite;

        }

        [System.Serializable]
        public class Favorite
        {
            public string assetGuid;
            public string cachedPath;
            public long localId;
            public List<int> pathCoords;
            public int instanceID;  
            public string sceneRefGuid;
            public int workspaceIdx;
            public int itemIdx;
        }
    }
}
