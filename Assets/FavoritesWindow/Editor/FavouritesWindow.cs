namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEditorInternal;
	using UnityEngine;

	public class FavouritesWindow : EditorWindow
	{
		private GUIStyle normalSelected_Focused;
		private GUIStyle normalSelected_Unfocused;
		private GUIStyle normalNotSelected;
		private GUIStyle errorSelected_Focused;
		private GUIStyle errorSelected_Unfocused;
		private GUIStyle errorNotSelected;

		[MenuItem( "Window/Favorites" )]
		public static void ShowWindow()
		{
			GetWindow().Show();
		}

		private static FavouritesWindow GetWindow()
		{
			var window = GetWindow<FavouritesWindow>();
			window.titleContent = new GUIContent(
				"Favorites",
				EditorEx.LoadPluginIcon("Star.png")
			);

			return window;
		}

		private void ClearWorkspace()
		{
			undo.CaptureStateBefore( "Clear Favorites Workspace" );
			workspaceGuiList.Clear();
			undo.CaptureStateAfter();
			GetWindow().Repaint();
		}

		[SerializeField]
		private Vector2 initialScrollPosition = Vector2.zero;

		private FavouritesUndo undo;

		private UnityGUIList<FavoriteItem> workspaceGuiList;
		private FavoritesPersistentState WindowPersistentState;
		private UnityIMGUI gui;
		private AbstractAssetDatabase assetDatabase = new UnityAssetDatabase();
		private SceneRefContainer sceneRefContainer;

		private bool projectDidUpdate = true;
		private bool hierarchyDidUpdate = true;
        private bool repaintRequested = true;

        private bool IsWindowFocused
		{
			get
			{
				return EditorWindow.focusedWindow == this;
			}
		}

		private void OnProjectChanged()
		{
			Debug.Log("On Project changed");
			this.projectDidUpdate = true;
            this.repaintRequested = true;
        }

		private void OnHierarchyChanged()
		{
			Debug.Log("On hierarchy changed");
			this.hierarchyDidUpdate = true;
            this.repaintRequested = true;
		}

		private void OnEnable()
		{
			#pragma warning disable 618
			EditorApplication.projectWindowChanged += OnProjectChanged;
			EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
			#pragma warning restore 618

			var favoritesSystem = EditorEx.GetOrCreate<FavoritesSystem>();
			sceneRefContainer = favoritesSystem.SceneRefContainer;
			WindowPersistentState = favoritesSystem.PersistentState;

			InitializeGUIStyles();
			this.maxSize = new Vector2( 10000, 10000 );
			this.minSize = new Vector2( 220, 100 );

			gui = new UnityIMGUI();
			undo = new FavouritesUndo( WindowPersistentState );
			workspaceGuiList = CreateWorkspaceGUIList();

			favoritesSystem.RegisterView(this.workspaceGuiList);
			EditorEx.RestorePopupDependencies( WindowPersistentState, undo );

			Debug.Log( "favorite enable!!" );

			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		private void OnDisable()
		{
			#pragma warning disable 618
			EditorApplication.projectWindowChanged -= OnProjectChanged;
			EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
			#pragma warning restore 618	

			Undo.undoRedoPerformed -= UndoRedoPerformed;
			Debug.Log( "favorite disable!!" );
		}

		private void OnDestroy()
		{
			var favoritesSystem = EditorEx.GetOrCreate<FavoritesSystem>();
			favoritesSystem.RegisterView(null);

			EditorEx.ClosePopups();
		}

		private void UndoRedoPerformed()
		{
			EditorEx.RepaintPopups();
		}

		private UnityGUIList<FavoriteItem> CreateWorkspaceGUIList()
		{
			var listEventDiscriminator = new ListEventDiscriminator( new EditorClock() );
            listEventDiscriminator.WhenDoubleTapped += OnItemWasDoubleTapped;
            listEventDiscriminator.WhenTapped += _ => repaintRequested = true;
            listEventDiscriminator.WhenDoubleTapped += _ => repaintRequested = true;
            listEventDiscriminator.WhenDragUpdated += () => repaintRequested = true;

            return new UnityGUIList<FavoriteItem>(
				DrawFavorite,
				DrawEmptyCase,
				( objectReference, objectPath ) =>
				{
					if(FavoriteItem.IsSceneRef(objectReference, objectPath, assetDatabase))
					{
						return sceneRefContainer.GetNew( objectReference, objectPath );
					}
					else
					{
						return new FavoriteItem( objectReference, objectPath, assetDatabase );
					}
				},
				gui,
				new DragAndDropUnity(),
				listEventDiscriminator,
				this.initialScrollPosition );
        }

		private void OnItemWasDoubleTapped( int itemIdx )
		{
			var items = workspaceGuiList.GetNormalItems();
			if ( itemIdx < 0 || itemIdx >= items.Count )
				return;

			var item = items[itemIdx].Value;
			if ( !item.isMissingReference )
			{
				AssetDatabase.OpenAsset( item.Asset );
			}
		}

		private void OnGUI()
		{
			Event e = Event.current;

			gui.OnGUIBegun();
			DrawHeader();
			workspaceGuiList.GetVisibleItems().SetMinimumConflictingDisplayPaths();

			bool listChanged = workspaceGuiList.DoList( 
				new GUIEvent( e ), 
				this.position.height );

			if ( listChanged )
			{
				// This one cannot use the Undo class since it requires the capture before state is stored
				Undo.RegisterCompleteObjectUndo( WindowPersistentState, "Favorites Workspace Modified" );
				WindowPersistentState.StoreCurrentWorkspace();
				EditorUtility.SetDirty( WindowPersistentState );
			}
			gui.OnGUIEnded();
			this.initialScrollPosition = workspaceGuiList.ScrollPosition;

			this.projectDidUpdate = false;
			this.hierarchyDidUpdate = false;
		}

		private void DrawHeader()
		{
			bool drawButtonsOnSameLine = position.width >= 300;
			Event guiEvent = Event.current;
			EditorGUILayout.BeginHorizontal();

			int workspaceIdx = WindowPersistentState.SelectedWorkspaceIdx;
			workspaceIdx = Mathf.Clamp( workspaceIdx, -1, WindowPersistentState.WorkspaceNames.Length - 1 );

			string selectedWorkspaceName =
				workspaceIdx != -1 ?
				WindowPersistentState.WorkspaceNames[workspaceIdx] :
				"Choose Workspace...";

			GenericMenu workspaceMenu = new GenericMenu();
			for ( int i = 0; i < WindowPersistentState.WorkspaceNames.Length; ++i )
			{
				string name = WindowPersistentState.WorkspaceNames[i];
				workspaceMenu.AddItem(
					new GUIContent( name ),
					i == WindowPersistentState.SelectedWorkspaceIdx,
					idx =>
					{
						int index = ( int )idx;
						if ( index != WindowPersistentState.SelectedWorkspaceIdx )
						{
							// Need to force this call so the undo does not register the workspace as it was before 
							// the guilist changes are applied
							undo.CaptureStateBefore( "Choose favorites workspace" );
							WindowPersistentState.SetSelectedWorkspace( index );
							undo.CaptureStateAfter();
							Debug.Log( "Popup changed!" );
						}
					},
					i );
			}
			workspaceMenu.AddSeparator( string.Empty );
			workspaceMenu.AddItem( new GUIContent( "Create New Workspace..." ), false, () =>
			{
				CreateWorkspacePopup.Show( WindowPersistentState, undo );
			} );

			GUILayout.Label( string.Empty, EditorStyles.toolbar, GUILayout.Width( 6 ) );
			workspaceMenu.AddItem( new GUIContent( "Delete Workspaces..." ), false, () =>
			{
				DeleteWorkspacePopup.Show( WindowPersistentState, undo );
			} );

			if ( GUILayout.Button( selectedWorkspaceName, EditorStyles.toolbarPopup ) )
				workspaceMenu.DropDown( new Rect( 6, 20, 0, 0 ) );

			if ( drawButtonsOnSameLine )
			{
				bool renameKeyPressed = guiEvent.isKey && guiEvent.keyCode == KeyCode.F2; 
				if ( GUILayout.Button( "Rename", EditorStyles.toolbarButton, GUILayout.MaxWidth( 100 ), GUILayout.MinWidth( 20 ) )
					|| renameKeyPressed )
				{
					RenameWorkspacePopup.Show( WindowPersistentState, undo );
					if(renameKeyPressed)
						guiEvent.Use();
				}

				if ( GUILayout.Button( "Clear", EditorStyles.toolbarButton, GUILayout.MaxWidth( 100 ), GUILayout.MinWidth( 20 ) ) )
					ClearWorkspace();
			}

			EditorGUILayout.EndHorizontal();
			if ( !drawButtonsOnSameLine )
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label( string.Empty, EditorStyles.toolbar, GUILayout.Width( 6 ) );
				bool renameKeyPressed = guiEvent.isKey && guiEvent.keyCode == KeyCode.F2;
				if ( GUILayout.Button( "Rename", EditorStyles.toolbarButton, GUILayout.MinWidth( 50 ) )
					|| renameKeyPressed )
				{
					RenameWorkspacePopup.Show( WindowPersistentState, undo );
					if(renameKeyPressed)
						guiEvent.Use();
				}

				if ( GUILayout.Button( "Clear", EditorStyles.toolbarButton, GUILayout.MinWidth( 50 ) ) )
					ClearWorkspace();
				EditorGUILayout.EndHorizontal();
			}
		}

		private GUILayoutOption[] xButtonLayoutOptions = new GUILayoutOption[]
		{
			GUILayout.Width( 20 )
		};

		private GUILayoutOption[] favoriteLabelOptions = new GUILayoutOption[]
		{
			GUILayout.Height( 20 ) 
		};

		private Rect DrawFavorite( ListItem<FavoriteItem> listItem )
		{
			FavoriteItem item = listItem.Value;
			if (item.ShouldReset(projectDidUpdate, hierarchyDidUpdate))
			{
				Debug.LogFormat("Resetting: {0}", item.FullVirtualPath);
				item.Reset();
			}

			GUILayout.BeginHorizontal();

			Texture icon = item.GetIcon();

			string path = item.isMissingReference ? item.AssetPath : item.DisplayPath;
			string normalTooltip = DragAndDrop.visualMode == DragAndDropVisualMode.None ? item.FullVirtualPath : string.Empty;
			string tooltip = item.isMissingReference ? item.FullVirtualPath + item.ErrorMessage : normalTooltip;
			string text = string.IsNullOrEmpty(path) ? "<Unknown last location>" : path;
			string title = item.DisplayPath;

			if (item.isMissingReference)
				text = string.Concat("Missing: ", text);

			GUIStyle style = GetItemStyle(listItem);

			EditorGUIUtility.labelWidth = 100;

			// Not using iconscope to support 5.5
			var iconSizeBefore = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(16 * Vector2.one);

			EditorGUILayout.LabelField(
				new GUIContent(title, icon, tooltip),
				style,
				favoriteLabelOptions);
			EditorGUIUtility.SetIconSize(iconSizeBefore);
			EditorGUIUtility.labelWidth = 0;

			Rect favoriteLabelRect = GUILayoutUtility.GetLastRect();
			if (GUILayout.Button("X", xButtonLayoutOptions))
			{
				undo.CaptureStateBefore("Delete Item From Favorites");
				workspaceGuiList.Remove(item);
				undo.CaptureStateAfter();
			};

			if (Event.current.type == EventType.Repaint && (item.IsSubAsset || item.IsSceneReference))
			{
				Texture subIcon = item.GetSubIcon();

				Rect iconRect = new Rect(
					favoriteLabelRect.x,
					favoriteLabelRect.y + 10,
					10,
					10);

				Color prevColor = GUI.color;
				GUI.color = new Color(.9f, .9f, .9f);
				GUI.DrawTexture(iconRect, subIcon, ScaleMode.ScaleToFit);

				GUI.color = prevColor;
			}

			GUILayout.EndHorizontal();

			return favoriteLabelRect;
		}

		private GUIStyle GetItemStyle( ListItem<FavoriteItem> listItem )
		{
			GUIStyle normalSelected = IsWindowFocused ? normalSelected_Focused : normalSelected_Unfocused;
			GUIStyle errorSelected = IsWindowFocused ? errorSelected_Focused : errorSelected_Unfocused;
			GUIStyle normal = listItem.IsSelected ? normalSelected : normalNotSelected;
			GUIStyle error = listItem.IsSelected ? errorSelected : errorNotSelected;

			return listItem.Value.isMissingReference ? error : normal;
		}

		private void InitializeGUIStyles()
		{
			GUISkin skin = GetCurrentSkin();

			// This is simple way to get the texture specific for each editor theme
			Texture2D selectedFocusedBg = skin.GetStyle( "Hi Label" ).onActive.background;
			Texture2D selectedUnfocusedBg = skin.GetStyle( "Hi Label" ).onNormal.background;

			normalNotSelected = new GUIStyle( skin.GetStyle( "LargeLabel" ) );
			normalSelected_Focused = new GUIStyle( normalNotSelected );
			normalSelected_Focused.normal.background = selectedFocusedBg;
			normalSelected_Focused.normal.textColor = Color.white;
			normalSelected_Focused.overflow = new RectOffset( 0, 0, 1, 1 );
			normalSelected_Unfocused = new GUIStyle( normalSelected_Focused );
			normalSelected_Unfocused.normal.background = selectedUnfocusedBg;



			var referenceErrorStyle = skin.GetStyle( "ErrorLabel" );
			errorNotSelected = new GUIStyle( normalNotSelected );
			errorNotSelected.normal.textColor = referenceErrorStyle.normal.textColor;
			errorSelected_Focused = new GUIStyle( errorNotSelected );
			errorSelected_Focused.normal.background = selectedFocusedBg;
			errorSelected_Focused.overflow = new RectOffset( 0, 0, 1, 1 );
			errorSelected_Unfocused = new GUIStyle( errorSelected_Focused );
			errorSelected_Unfocused.normal.background = selectedUnfocusedBg;
		}

		private static GUISkin GetCurrentSkin()
		{
			return EditorGUIUtility.GetBuiltinSkin( EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector );
		}

		private void DrawEmptyCase()
		{
			float width = EditorGUIUtility.currentViewWidth;
			float lineHeight = EditorGUIUtility.singleLineHeight;
			float height = position.height - EditorGUIUtility.singleLineHeight * 2;
			var rect = new Rect(
				0,
				height / 2 - lineHeight,
				width,
				lineHeight * 2 );

			GUILayout.BeginArea( rect );
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( "Drop assets here to add..." );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( "You can also Drag&Drop to reorder" );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		public void Update()
		{
			repaintRequested = repaintRequested || AnyIconRefreshed();
			
			if (!repaintRequested)
				return;

			repaintRequested = false;

			Repaint();
		}

		private bool AnyIconRefreshed()
		{
			bool anyUpdated = false;
			var visibleItems = workspaceGuiList.GetVisibleItems();
			foreach (var item in visibleItems)
			{
				bool iconRefreshed;
				item.Value.RefreshIcon(out iconRefreshed);
				anyUpdated = anyUpdated || iconRefreshed;
			}

			if(anyUpdated)
				Debug.Log("Request repaint by icon update");

			return anyUpdated;
		}

		private void OnFocus()
		{
		}

		private void OnLostFocus()
		{
			workspaceGuiList.OnLostFocus();
            this.repaintRequested = true;
        }

		public class FavouritesUndo
		{
			private FavoritesPersistentState favoritesState;

			public FavouritesUndo(
				FavoritesPersistentState favoritesState )
			{
				this.favoritesState = favoritesState;
			}

			public void CaptureStateBefore( string undoName )
			{
				favoritesState.StoreCurrentWorkspace();
				Debug.LogFormat( "StateBeforeUndo: '{0}'", favoritesState.DebugString() );
				Undo.RegisterCompleteObjectUndo( favoritesState, undoName );
			}

			public void CaptureStateAfter()
			{
				Debug.LogFormat( "StateBeforeSetDirty: '{0}'", favoritesState.DebugString() );
				favoritesState.StoreCurrentWorkspace();
				EditorUtility.SetDirty( favoritesState );
			}

			public void ClearAll()
			{
				Undo.ClearUndo( favoritesState );
			}
		}
	}
}
