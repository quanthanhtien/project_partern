namespace Favorites
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public class FavoritesSystem : ScriptableObject 
	{
        private FavoritesPersistentState favoritesPersistentState;
		private SceneRefContainer sceneRefContainer;
        private UnityGUIList<FavoriteItem> view;

        private static string FavoritesPrefsKey
		{
			get
			{
				return PlayerSettings.productGUID + "_Favorites_Workspaces_Key";
			}
		}

		[InitializeOnLoadMethod]
		public static void OnEditorApplicationLoaded()
		{
			EditorEx.GetOrCreate<FavoritesSystem>();
		}

		public FavoritesPersistentState PersistentState
		{
			get
			{
				return this.favoritesPersistentState;
			}
		}

		public SceneRefContainer SceneRefContainer
		{
			get
			{
				return this.sceneRefContainer;
			}
		}

		public void RegisterView(UnityGUIList<FavoriteItem> guiList)
		{
			this.view = guiList;

			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		public void OnEnable()
		{
			this.hideFlags = HideFlags.DontSave;

			var storage = new FavoriteWindowEditorStorage( FavoritesPrefsKey ) ;
			var assetDatabase = new UnityAssetDatabase();
			this.favoritesPersistentState = EditorEx.GetOrCreate<FavoritesPersistentState>();
			this.sceneRefContainer = EditorEx.GetOrCreate<SceneRefContainer>();
			favoritesPersistentState.SetDependencies( storage, sceneRefContainer, assetDatabase );
			favoritesPersistentState.Restore();
			sceneRefContainer.SetDependendcies(favoritesPersistentState, assetDatabase);			
		}

		public void OnUpdate()
		{
			favoritesPersistentState.RegisterView(view);
			EditorApplication.update -= OnUpdate;
		}

		public void OnDisable()
		{
			favoritesPersistentState.Save();
			EditorApplication.update -= OnUpdate;
		}
	}
	
}
