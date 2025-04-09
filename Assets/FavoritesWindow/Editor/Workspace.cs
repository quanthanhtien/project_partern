namespace Favorites
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	// It has manual serialization to allow it to be stored as private workspace ( ie. stored as user prefs )
	// and it is serializable to support undo 
	[Serializable]
	public class Workspace
	{
		private const char guidSepparator = ',';
		private const char nameSepparator = ':';
		private const char workspaceSepparator = ';';
		private const char pathCoordsSepparator = '|';

		[SerializeField]
		public string name;

		[SerializeField]
		public List<string> itemIds;
		[SerializeField]
		public List<int> instanceIds = new List<int>();
		[SerializeField]
		public List<string> sceneRefGuids = new List<string>();
		[SerializeField]
		public List<long> localIds = new List<long>();
		[SerializeField]
		public List<PathCoordinates> pathCoordinates = new List<PathCoordinates>();

		[SerializeField]
		public List<string> selectedItemIds = new List<string>();

		[SerializeField]
		public List<string> cachedItemPaths = new List<string>();

		public static Workspace Create( string name, params string[] guids )
		{
			var ws = new Workspace();
			ws.name = name;
			ws.itemIds = new List<string>( guids );
			ws.instanceIds = new List<int>( guids.Length );
			ws.sceneRefGuids = new List<string>(guids.Length);
			ws.localIds = new List<long> (guids.Length);
			ws.pathCoordinates = new List<PathCoordinates> (guids.Length);
			for (int i = 0; i < guids.Length; i++)
			{
				ws.instanceIds.Add(0);
				ws.localIds.Add (-1L);
				ws.sceneRefGuids.Add(string.Empty);
				ws.pathCoordinates.Add (new PathCoordinates ());
			}

			return ws;
		}

		private Workspace()
		{
			
		}

		private string Serialize()
		{
			// Serialization format 
			// - Asterisk before a guid means selected 
			// <name>:<guid1>,<*guid2>,<guid3>,...:<cachedPath1>,<cachedPath2>,...
			// ...:<localId1>,<localId2>,<localId3>,
			// ...:<pathcoord1_1>|<pc1_2>|<pc1_3>,<pc2_1>|<pc2_2>|<pc2_3>,...
			return string.Concat(
				name,
				nameSepparator,
				string.Join(
					guidSepparator.ToString(),
					itemIds.Select( id => ( selectedItemIds.Contains( id ) ? "*" + id : id ) )
						   .ToArray() )
				,
				nameSepparator,
				string.Join(
					guidSepparator.ToString(),
					cachedItemPaths.Where( p => !string.IsNullOrEmpty( p ) ).ToArray() ),
				nameSepparator,
				string.Join(
					guidSepparator.ToString(), 
					localIds.Select( id => id.ToString()).ToArray()),
				nameSepparator,
				string.Join(
					guidSepparator.ToString(),
					pathCoordinates.Select( 
						pc => string.Join(
							pathCoordsSepparator.ToString(),
							pc.Coords.Select(index => index.ToString())
								 						   .ToArray()
					)).ToArray()
				) );
		}

		public static string SerializeMany( List<Workspace> workspaces )
		{
			// Serializetion format
			// <serializedWorkspace1>;<serializedWorkspace2>;<serializedWorkspace3>;...
			string[] serializedWorkspaces = new string[workspaces.Count];
			for ( int i = 0; i < serializedWorkspaces.Length; i++ )
			{
				serializedWorkspaces[i] = workspaces[i].Serialize();
			}

			return string.Join(
				workspaceSepparator.ToString(),
				serializedWorkspaces );
		}

		private static Workspace FromSerializedString( string serializedWorkspace )
		{
			var tempSplit = serializedWorkspace.Split( nameSepparator );
			string name = tempSplit[0];
			string jointGuids = tempSplit[1];
			string jointCachedPaths = tempSplit.Length > 2 ? tempSplit[2] : string.Empty;
			string jointLocalIds = tempSplit.Length > 3 ? tempSplit [3] : string.Empty;
			string jointPathCoords = tempSplit.Length > 4 ? tempSplit [4] : string.Empty;

			var rawGuids = string.IsNullOrEmpty( jointGuids ) ? new string[0] : jointGuids.Split( guidSepparator );
			var cachedPaths = string.IsNullOrEmpty( jointCachedPaths ) ? new string[0] : jointCachedPaths.Split( guidSepparator );
			var allguids = rawGuids.Select( id => id.TrimStart( '*' ) );
			var selectedguids = rawGuids.Where( id => id.StartsWith( "*" ) )
										.Select( id => id.TrimStart( '*' ) );
			var localIds = string.IsNullOrEmpty( jointLocalIds ) ?
				Enumerable.Repeat(-1L,rawGuids.Length) : 
				jointLocalIds.Split( guidSepparator )
							 .Select(id => long.Parse(id));

			var pathCoords = string.IsNullOrEmpty(jointPathCoords)?
				rawGuids.Select( _ =>  new PathCoordinates()):
				jointPathCoords.Split(guidSepparator)
							   .Select( pcs => new PathCoordinates()
							   {
								   Coords = pcs.Split(pathCoordsSepparator)
											   .Where(pathIdx => !string.IsNullOrEmpty(pathIdx))
									   		   .Select(pathIdx => int.Parse(pathIdx))
											   .ToList()
							   });

			int itemsCount = rawGuids.Length;
			var workspace = new Workspace
			{
				itemIds = new List<string>( allguids ),
				selectedItemIds = new List<string>( selectedguids ),
				cachedItemPaths = new List<string>( cachedPaths ),
				localIds = new List<long>(localIds),
				pathCoordinates = new List<PathCoordinates>(pathCoords),
				instanceIds = new List<int>(itemsCount),
				sceneRefGuids = new List<string>(itemsCount),

				name = name
			};

			for (int i = 0; i < itemsCount; i++)
			{
				workspace.instanceIds.Add(0);
				workspace.sceneRefGuids.Add(string.Empty);
			}

			return workspace;
		}

		public static List<Workspace> GetAll( string serializedWorkspaces )
		{
			var serializedWorkspaceArray = serializedWorkspaces.Split( workspaceSepparator );
			return serializedWorkspaceArray
				.Where( text => !string.IsNullOrEmpty( text ) )
				.Select( text => FromSerializedString( text ) )
				.ToList();
		}
	}

	[Serializable]
	public class PathCoordinates
	{
		public List<int> Coords = new List<int>();
	}

}