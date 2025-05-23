using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileData
{
    public int x;
    public int y;
    public string tileGUID;
}

[System.Serializable]
public class MapSaveData
{
    public List<SceneField> revealedScenes = new();
    public List<TileData> revealedTiles = new();
}
public class MapRoomManager : MonoBehaviour
{
    public static MapRoomManager instance;

    public Tilemap masterMapTilemap;

    public List<TileBase> tilePalette;
    private Dictionary<string, TileBase> tileLookup;

    private MapSaveData mapSaveData;

    public bool useSavedMap;

    private void Awake()
    {
        if (instance == null) instance = this;

        tileLookup = new Dictionary<string, TileBase>();
        foreach (var tile in tilePalette)
        {
            tileLookup[tile.name] = tile;
        }
        if (useSavedMap)
            mapSaveData = SaveManager.GetMapFromSave();
        else
            mapSaveData = new MapSaveData();

        RestoreMapFromSave();
    }

    public void RestoreMapFromSave()
    {
        foreach (var tileData in mapSaveData.revealedTiles)
        {
            Vector3Int pos = new Vector3Int(tileData.x, tileData.y, 0);
            TileBase tile = tileLookup.ContainsKey(tileData.tileGUID) ? tileLookup[tileData.tileGUID] : null;
            masterMapTilemap.SetTile(pos, tile);
        }

    }
    public void RevealRoom()
    {
        foreach (var room in Resources.FindObjectsOfTypeAll<MapContainerData>())
        {
            if (!room.gameObject.scene.isLoaded)
                continue;

            if (!room.hasBeenRevealed)
            {
                Tilemap roomTileMap = room.GetComponent<Tilemap>();
                CopyTilesToMasterMap(roomTileMap);
                room.hasBeenRevealed = true;
                mapSaveData.revealedScenes.Add(room.roomScene);
                SaveTilesFromTilemap(roomTileMap);
                SaveManager.SaveMapData(mapSaveData);
            }
        }
    }

    public void CopyTilesToMasterMap(Tilemap source)
    {
        BoundsInt bounds = source.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = source.GetTile(pos);
            if (tile != null)
                masterMapTilemap.SetTile(pos, tile);
        }
    }

    public void SaveTilesFromTilemap(Tilemap source)
    {
        BoundsInt bounds = source.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = source.GetTile(pos);
            if (tile != null)
            {
                string guid = tile.name;

                mapSaveData.revealedTiles.Add(new TileData
                {
                    x = pos.x,
                    y = pos.y,
                    tileGUID = guid
                });
            }
        }
    }

}