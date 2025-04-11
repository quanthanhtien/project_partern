namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	public static class EditorEx
	{
		private static bool supportsSetFolderSelection;

        private static string pluginRootFolder;

		private static Type projectBrowserType;
		private static MethodInfo setFolderSelectionMethod;
		private static FieldInfo viewModeField;
		private static MethodInfo clearSearchMethod;
        private static MethodInfo initMethod;
        private static FieldInfo assetTreeField;
		private static MethodInfo assetTreeSetSelectionMethod;
		private static FieldInfo listAreaField;
		private static MethodInfo listAreaInitSelectionMethod;
		private static PropertyInfo isLockedProperty;
		private static FieldInfo isLockedField;
		private static int ViewMode_OneColumn = 0;

		static EditorEx()
		{
			supportsSetFolderSelection = false;

			projectBrowserType = typeof( Editor ).Assembly.GetType( "UnityEditor.ProjectBrowser" );

			if ( projectBrowserType == null )
				return;

            setFolderSelectionMethod = FindInstanceMethod(
                projectBrowserType, 
                "SetFolderSelection", 
                typeof(int[]), typeof(bool));

			if ( setFolderSelectionMethod == null )
				return;

			viewModeField =
				projectBrowserType.GetField(
					"m_ViewMode",
					BindingFlags.Instance | BindingFlags.NonPublic );

			if ( viewModeField == null )
				return;

            clearSearchMethod = FindInstanceMethod(
                projectBrowserType,
                "ClearSearch");

			initMethod = FindInstanceMethod(
				projectBrowserType,
				"Init");

			assetTreeField = 
				projectBrowserType.GetField(
					"m_AssetTree",
					BindingFlags.Instance | BindingFlags.NonPublic);

			assetTreeSetSelectionMethod = FindInstanceMethod(
                assetTreeField.FieldType,
                "SetSelection",
                typeof(int[]), typeof(bool));
			
			listAreaField = 
				projectBrowserType.GetField(
					"m_ListArea",
					BindingFlags.Instance | BindingFlags.NonPublic );
			
			listAreaInitSelectionMethod = FindInstanceMethod(
                listAreaField.FieldType,
                "InitSelection");

			// TODO: add warning message if in some future version the required methods are missing

			// Notes
			// Unity 55 - uses private field 'm_IsLocked' to hold the state
			// After 2018.1.0a3(included) - uses internal property 'isLocked' to hold the state

			isLockedProperty = 
				projectBrowserType.GetProperty(
					"isLocked",
					BindingFlags.Instance | BindingFlags.NonPublic );

			isLockedField = 
				projectBrowserType.GetField(
					"m_IsLocked",
					BindingFlags.Instance | BindingFlags.NonPublic );

			if( isLockedField == null && isLockedProperty == null)
				UnityEngine.Debug.LogWarning("ProjectBrowser.IsLocked access has been refactored by Unity Team, please contact Favorites Window author for a fix.");
			
			supportsSetFolderSelection = true;
		}

        public static string GetPluginRootFolder()
        {
            if(pluginRootFolder == null)
                pluginRootFolder = FindPluginRootFolder();
            
            return pluginRootFolder;
        }

		public static Texture2D LoadPluginIcon( string relativeFilename )
		{
			string filename = string.Format( "{0}/Editor/{1}", GetPluginRootFolder(), relativeFilename );
			return AssetDatabase.LoadAssetAtPath<Texture2D>( filename );
		}

		/// <summary>
		/// Tries to get a window of type T
		/// </summary>
		/// <typeparam name="T">The type of the window to retreive</typeparam>
		/// <returns>The first found instance of the windwo, null otherwise</returns>
		public static T TryGetWindow<T>() where T : UnityEngine.Object
		{
			// NOTE(rafa): Unity does the same for GetWindow internal code
			var windows = Resources.FindObjectsOfTypeAll<T>();
			if ( windows.Length > 0 )
				return windows[0];
			else
				return null;
		}

		/// <summary>
		/// Gets the first found T in memory, or creates a new one if not found
		/// </summary>
		/// <typeparam name="T">The type of object</typeparam>
		/// <returns>An insance of T</returns>
		public static T GetOrCreate<T>() where T : ScriptableObject
		{
			var instance = TryGetWindow<T>();
			if ( instance == null )
				instance = ScriptableObject.CreateInstance<T>();

			return instance;
		}

		public static void RestorePopupDependencies( FavoritesPersistentState favoritesState, FavouritesWindow.FavouritesUndo undo )
		{
			var rename = TryGetWindow<RenameWorkspacePopup>();
			if ( rename != null )
				rename.SetDependencies( favoritesState, undo );

			var create = TryGetWindow<CreateWorkspacePopup>();
			if ( create != null )
				create.SetDependencies( favoritesState, undo );

			var delete = TryGetWindow<DeleteWorkspacePopup>();
			if ( delete != null )
				delete.SetDependencies( favoritesState, undo );
		}

		public static void ClosePopups()
		{
			var rename = TryGetWindow<RenameWorkspacePopup>();
			if ( rename != null )
				rename.Close();

			var create = TryGetWindow<CreateWorkspacePopup>();
			if ( create != null )
				create.Close();

			var delete = TryGetWindow<DeleteWorkspacePopup>();
			if ( delete != null )
				delete.Close();
		}

		public static void RepaintPopups()
		{
			var rename = TryGetWindow<RenameWorkspacePopup>();
			if ( rename != null )
				rename.Repaint();

			var create = TryGetWindow<CreateWorkspacePopup>();
			if ( create != null )
				create.Repaint();

			var delete = TryGetWindow<DeleteWorkspacePopup>();
			if ( delete != null )
				delete.Repaint();
		}

		public static bool IsSelectAll( this Event e )
		{
			return e.type == EventType.ExecuteCommand && e.commandName == "SelectAll";
		}

		public static void AcceptSelectAll( this Event e )
		{
			if ( e.type == EventType.ValidateCommand && e.commandName == "SelectAll" )
				e.Use();
		}

		/// <summary>
		/// Shows the combined folders content in two column browser
		/// This function expects all the objects to be folders, does not validate input
		/// </summary>
		/// <param name="folders">The folders to show</param>
		public static void ShowFolderContents( params UnityEngine.Object[] folders )
		{
			if ( !supportsSetFolderSelection )
			{
				Selection.objects = folders;
				return;
			}

			var folderIds = new int[folders.Length];
			for ( int i = 0; i < folderIds.Length; i++ )
			{
				folderIds[i] = folders[i].GetInstanceID();
			}
			SetFolderSelection( folderIds );
		}

		private static void SetFolderSelection( params int[] folderIds )
		{
			UnityEngine.Object[] projectBrowsers = Resources.FindObjectsOfTypeAll( projectBrowserType );

			List<UnityEngine.Object> oneColumnBrowsers = new List<UnityEngine.Object>(projectBrowsers.Length);
			List<UnityEngine.Object> twoColumnBrowsers = new List<UnityEngine.Object>(projectBrowsers.Length);

			for ( int i = 0; i < projectBrowsers.Length; i++ )
			{
				var browser = projectBrowsers[i];

                if(initMethod != null)
                    initMethod.Invoke(browser, null);
				if( IsBrowserLocked(browser) == true )
					continue;

				if ( ( int )viewModeField.GetValue( browser ) == ViewMode_OneColumn )
					oneColumnBrowsers.Add(browser);
				else
					twoColumnBrowsers.Add(browser);
			}

			for ( int i = 0; i < oneColumnBrowsers.Count; i++ )
			{
				var browser = oneColumnBrowsers[i] as EditorWindow;
				object treeView = assetTreeField.GetValue(browser);
				object listArea = listAreaField.GetValue(browser); 

				clearSearchMethod.Invoke(browser, null);

				listAreaInitSelectionMethod.Invoke(
					listArea,
					new object[]{folderIds}
				);

				bool revealSelectionAndFrameLastSelected = true;
				assetTreeSetSelectionMethod.Invoke(
					treeView,
					new object[]
					{
						folderIds, 
						revealSelectionAndFrameLastSelected 
					}
				);

				browser.Repaint();
			}

			for ( int i = 0; i < twoColumnBrowsers.Count; i++ )
			{
				var browser = twoColumnBrowsers[i];

				bool revealAndFrameInFolderTree = true;
				setFolderSelectionMethod.Invoke(
					browser, new object[]
					{
						folderIds,
						revealAndFrameInFolderTree
					} );
			}
		}

		private static bool IsBrowserLocked(System.Object browser)
		{
			if( isLockedField != null)
				return (bool)isLockedField.GetValue(browser);
			else if ( isLockedProperty != null)
				return (bool)isLockedProperty.GetValue(browser, null);
			else
				return false;
		}

        private static string FindPluginRootFolder()
        {
            string[] candidateFolders = AssetDatabase.FindAssets("t:DefaultAsset FavoritesWindow");

            for( int i = 0; i < candidateFolders.Length; i++ )
            {
                string guid = candidateFolders[i];
                string candidateAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetsFolderAbsolutePath = Application.dataPath;

                string candidateAbsolutePath = string.Concat(
                    assetsFolderAbsolutePath.TrimEnd("Assets".ToCharArray()),
                    candidateAssetPath
                );

                var attributes = File.GetAttributes(candidateAbsolutePath);
                bool isFolder = (attributes & FileAttributes.Directory) == FileAttributes.Directory;
                if(isFolder)
                {
                    string[] subfolders = Directory.GetDirectories(candidateAbsolutePath, "Editor");
                    if( subfolders.Length > 0)
                    {
                        string[] files = Directory.GetFiles(subfolders[0], "FavouritesWindow.cs");
                        if(files.Length > 0)
                        {
                            return candidateAssetPath;
                        }
                    }
                }
            }

            return null;
        }

        private static MethodInfo FindInstanceMethod(Type type, string methodName, params Type[] argTypes)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                if(method.Name == methodName)
                {
                    var methodParams = method.GetParameters();
                    if( argTypes.Length == methodParams.Length )
                    {
                        bool fullMatch = true;
                        for (int argIdx = 0; argIdx < argTypes.Length; argIdx++)
                        {
                            fullMatch = fullMatch && methodParams[argIdx].ParameterType == argTypes[argIdx]; 
                        }

                        if (fullMatch)
                            return method;
                    }
                }
            }

            return null;
        }
    }
}