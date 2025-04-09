namespace Favorites
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
	using Object = UnityEngine.Object;

	[System.Serializable]
    public abstract class AbstractAssetDatabase
	{
		public abstract string GUIDToAssetPath( string guid );
		public abstract string AssetPathToGUID( string path );
		public abstract T LoadAssetAtPath<T>( string filename ) where T : Object;
		public abstract string GetAssetPath (UnityEngine.Object reference);
		public abstract bool Exists( string filename );
        public abstract Texture GetCachedIcon(string assetPath);
		public abstract Texture2D CopyTexture(Texture2D src);
        public abstract List<int> GetSceneRefCoords(UnityEngine.Object reference);
		public abstract bool TryGetLocalIdAndGuid(UnityEngine.Object instance, out long localId, out string guid);
		public abstract SceneRefResult TryGetSceneRef(			 
			string parentScenePath,
			string expectedSubpath,
			List<int> sceneCoords,
			out string scenePath, out UnityEngine.Object reference);
		public abstract T LoadAssetAtPath<T> ( string guid, long localId) where T : Object;
        public abstract string GetSceneRefFullPath(UnityEngine.Object reference);
		public abstract int GetInstanceId(UnityEngine.Object reference);
		public abstract UnityEngine.Object InstanceIDToObject(int instanceID);
		public abstract void GetDontDestroyOnLoadObjects(List<GameObject> result);
		public abstract int GetLoadedScenesCount();
		public abstract Scene GetLoadedSceneAt(int index);
    }

	public enum SceneRefResult
	{
		Success, MissingScene, SceneNotLoaded, GameobjectNotFound
	}

	[System.Serializable]
	public class UnityAssetDatabase : AbstractAssetDatabase
	{
		private SceneRefSpecific sceneRefMethods;

		public UnityAssetDatabase()
		{
			this.sceneRefMethods = new SceneRefSpecific(this);
		}

		public override bool Exists( string filename )
		{
			return File.Exists( filename ) || Directory.Exists( filename );
		}

		public override string GUIDToAssetPath( string guid )
		{
			return AssetDatabase.GUIDToAssetPath( guid );
		}

		public override string AssetPathToGUID( string path)
		{
			return AssetDatabase.AssetPathToGUID(path);
		}

		public override T LoadAssetAtPath<T>( string filename )
		{
			return AssetDatabase.LoadAssetAtPath<T>( filename );
		}

		public override string GetAssetPath( UnityEngine.Object reference )
		{
			return AssetDatabase.GetAssetOrScenePath (reference);
		}

        public override Texture GetCachedIcon(string assetPath)
        {
            return AssetDatabase.GetCachedIcon(assetPath);
        }

		public override Texture2D CopyTexture(Texture2D src)
		{
			bool mipmap = false;
			bool linear = false;
			Texture2D texture = new Texture2D(src.width, src.height, src.format, mipmap, linear);
			Graphics.CopyTexture(src, texture);
			return texture; 
		}

        public override List<int> GetSceneRefCoords(UnityEngine.Object reference)
		{
			return sceneRefMethods.GetSceneRefCoords(reference);
		}

        public override bool TryGetLocalIdAndGuid(UnityEngine.Object instance, out long localId, out string guid)
        {
			PropertyInfo inspectorModeInfo =
				typeof(SerializedObject).GetProperty(
					"inspectorMode", 
					BindingFlags.NonPublic | BindingFlags.Instance);
			
			SerializedObject serializedObject = new SerializedObject(instance);
			inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
			SerializedProperty localIdProp =
			serializedObject.FindProperty("m_LocalIdentfierInFile");   //note the misspelling!
 
			localId = localIdProp.longValue;

			var path = AssetDatabase.GetAssetPath(instance);
			guid = AssetDatabase.AssetPathToGUID(path);

            return true;
        }

        public override T LoadAssetAtPath<T>(string guid, long localId)
        {
            string path = GUIDToAssetPath(guid);
			Object [] assets = AssetDatabase.LoadAllAssetsAtPath(path);
			for( int i = 0; i < assets.Length; i++)
			{
				var asset = assets[i];
				if( asset is T)
				{
					long candidateLocalId;
					string candidateGuid;
					// Note(Rafa) make an overload that does not return guid, 
					// we are creating strings here for nothing) 
					// are we?? is guid cached internally by unity??
					TryGetLocalIdAndGuid(asset, out candidateLocalId, out candidateGuid);

					if( candidateLocalId == localId)
					{
						return asset as T;
					}
				}
			}

			return null;
        }

        public override SceneRefResult TryGetSceneRef( 
			string parentScenePath,
			string expectedPathInScene,
			List<int> sceneCoords,
			out string scenePath, out UnityEngine.Object reference)
        {
			return sceneRefMethods.TryGetSceneRef(
				parentScenePath,
				expectedPathInScene,
				sceneCoords,
				out scenePath,
				out reference);
        }

        public override string GetSceneRefFullPath(UnityEngine.Object reference)
		{
			return sceneRefMethods.GetSceneRefFullPath(reference);
		}

        public override int GetInstanceId(Object reference)
        {
            return reference.GetInstanceID();
        }

        public override Object InstanceIDToObject(int instanceID)
        {
            return EditorUtility.InstanceIDToObject(instanceID);
        }

		public override void GetDontDestroyOnLoadObjects(List<GameObject> result)
		{
			result.Clear();
			if(EditorApplication.isPlaying == false)
				return;

			GameObject temp = null;
			try
			{
				temp = new GameObject();
				UnityEngine.Object.DontDestroyOnLoad( temp );
				UnityEngine.SceneManagement.Scene dontDestroyOnLoad = temp.scene;
				UnityEngine.Object.DestroyImmediate( temp );
				temp = null;
		
				dontDestroyOnLoad.GetRootGameObjects(result);
			}
			finally
			{
				if( temp != null )
					UnityEngine.Object.DestroyImmediate( temp );
			}
		}

		public override int GetLoadedScenesCount()
		{
#if UNITY_2022_2_OR_NEWER
			return SceneManager.loadedSceneCount;
#else
			return EditorSceneManager.loadedSceneCount;
#endif
		}

		public override Scene GetLoadedSceneAt(int index)
		{
#if UNITY_2022_2_OR_NEWER
			return SceneManager.GetSceneAt(index);
#else
			return EditorSceneManager.GetSceneAt(index);
#endif
		}
	}

	[System.Serializable]
    public class SceneRefSpecific
    {
		private const string dontDestroyOnLoadScene = "DontDestroyOnLoad";
		private List<GameObject> rootObjects = new List<GameObject>();

		private AbstractAssetDatabase assetDatabase;

		public SceneRefSpecific(AbstractAssetDatabase assetDatabase)
		{
			this.assetDatabase = assetDatabase;
		}

        public List<int> GetSceneRefCoords(Object reference)
        {
            var pathCoords = new List<int>();
			var go = reference as GameObject;
			if( go == null && reference is Component)
				go = ((Component)reference).gameObject;

			if( go == null)
			 	throw new System.ArgumentOutOfRangeException("reference");
			
			Transform child = go.transform;
			Transform parent = child.parent;
			pathCoords.Add(child.GetSiblingIndex());
			while(parent != null)
			{
				child = parent;
				parent = child.parent;
				int childIdx = child.GetSiblingIndex();
				pathCoords.Add(childIdx);
			}

			pathCoords.Reverse();

			return pathCoords;
        }

        public SceneRefResult TryGetSceneRef(
			string parentScenePath,
			string expectedPathInScene, 
			List<int> sceneCoords, 
			out string scenePath, out Object reference)
        {
            bool isDontDestroyScene = dontDestroyOnLoadScene == parentScenePath;
			SceneAsset sceneAsset = assetDatabase.LoadAssetAtPath<SceneAsset>(parentScenePath);

			if(sceneAsset == null && !isDontDestroyScene )
			{
				scenePath = string.Empty;
				reference = null;
				return SceneRefResult.MissingScene;
			}

			rootObjects.Clear();
			bool sceneIsLoaded = false;

			if(isDontDestroyScene)
			{
				assetDatabase.GetDontDestroyOnLoadObjects(rootObjects);
				sceneIsLoaded = rootObjects.Count > 0;
			}
			else
			{
				for( int i=0; i<assetDatabase.GetLoadedScenesCount();i++)
				{
					var scene = assetDatabase.GetLoadedSceneAt(i);
					if( scene.path == parentScenePath )
					{
						scene.GetRootGameObjects(rootObjects);
						sceneIsLoaded = true;
						break;
					}
				}
			}

			if( !sceneIsLoaded )
			{
				scenePath = string.Empty;
				reference = null;
				return SceneRefResult.SceneNotLoaded;
			}

			var sb = new StringBuilder(50);
			int childIdx = sceneCoords[0];
			var transform = 
			childIdx < rootObjects.Count ? rootObjects[childIdx].transform : null;
			if( transform != null)
				sb.Append(transform.name);

			for( int i = 1; i<sceneCoords.Count && transform != null; i++)
			{
				sb.Append("/");
				childIdx = sceneCoords[i];
				transform = childIdx < transform.childCount ? transform.GetChild(childIdx) : null;
				if(transform != null)
					sb.Append(transform.name);
			}

			// Not found by coordinates but expected path exists
			if( transform == null && !string.IsNullOrEmpty(expectedPathInScene) )
            {
                // Try find by name
                transform = FindSceneRefByPath(expectedPathInScene, rootObjects);

                if (transform == null)
                {
                    // Fetch failed
                    scenePath = string.Empty;
                    reference = null;
                    return SceneRefResult.GameobjectNotFound;
                }
				else
				{
					scenePath = expectedPathInScene;
					reference = transform.gameObject;
					return SceneRefResult.Success;
				}
            }
			else if( transform == null )
			{
				// Fetch failed and no possible fallback
				scenePath = string.Empty;
				reference = null;
				return SceneRefResult.GameobjectNotFound;
			}

            string actualSubpath = sb.ToString();
			if(!string.IsNullOrEmpty(expectedPathInScene) 
			&& !expectedPathInScene.EndsWith(actualSubpath))
			{
				// Fetch mismatch, Try find by name or fail
				transform = FindSceneRefByPath(expectedPathInScene, rootObjects);
				if( transform != null )
				{
					scenePath = expectedPathInScene;
					reference = transform.gameObject;
					return SceneRefResult.Success;
				}
				else
				{
					// Fetch failed
					scenePath = string.Empty;
					reference = null;
					return SceneRefResult.GameobjectNotFound;
				}
			}

			scenePath = actualSubpath;
			reference = transform.gameObject;
			return SceneRefResult.Success;
        }

        public string GetSceneRefFullPath(Object reference)
        {
            var coords = GetSceneRefCoords(reference);
			Scene rootScene;
			var go = reference as GameObject;
			if(go != null)
				rootScene = go.scene;
			else
			{
				var component = reference as Component;
				rootScene = component.gameObject.scene;
			}

			var sb = new StringBuilder();
			sb.Append(rootScene.path).Append("/");
			rootObjects.Clear();

			if(rootScene.path == dontDestroyOnLoadScene)
			{
				assetDatabase.GetDontDestroyOnLoadObjects(rootObjects);
			}
			else
			{
				int sceneCount = assetDatabase.GetLoadedScenesCount();
				for( int i=0; i<sceneCount;i++)
				{
					var scene = assetDatabase.GetLoadedSceneAt(i);
					if( scene.path == rootScene.path )
					{
						scene.GetRootGameObjects(rootObjects);
						break;
					}
				}
			}

			Transform t = rootObjects[coords[0]].transform;

			sb.Append(t.name);
			if(coords.Count > 1)
			{
				sb.Append("/");
				for( int i = 1; i < coords.Count -1; i++)
				{
					t = t.GetChild(coords[i]);
					sb.Append(t.name)
						.Append("/");

				}
			
				t = t.GetChild(coords[coords.Count -1]);
				sb.Append(t.name);
			}

			return sb.ToString();
        }

        private Transform FindSceneRefByPath(string expectedPathInScene, List<GameObject> rootObjects)
        {
			Transform transform = null;
            var segments = expectedPathInScene.Split('/');
            if (segments.Length > 0)
            {
                for (int i = 0; i < rootObjects.Count; i++)
                {
                    var go = rootObjects[i];
                    if (go.name == segments[0])
                    {
                        transform = go.transform;
                        break;
                    }
                }

                for (int i = 1; i < segments.Length && transform != null; i++)
                {
                    transform = GetChildByName(transform, segments[i]);
                }
            }

            return transform;
        }

		private Transform GetChildByName(Transform parent, string childName)
		{
			int childCount = parent.childCount;
			for( int i = 0; i< childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if( child.name == childName)
					return child;
			}

			return null;
		}
    }
}