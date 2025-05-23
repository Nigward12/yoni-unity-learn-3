using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine.Tilemaps;
public static class SaveManager
{
    private static string mapDataSavePath => Application.persistentDataPath + "/mapdata.json";
    public static void SaveInCheckpoint(Checkpoint spawnCheckpoint)
    {

        PlayerManager.instance.SavePlayerData();

        PlayerPrefs.SetString(GameManager.SAVED_LEVEL_KEY, LoadingManager.instance.currentLevelData.levelName);

        PlayerPrefs.SetString(GameManager.SAVED_CHECKPOINT_KEY, spawnCheckpoint.name);

        PlayerPrefs.Save();
    }

    public static void SaveMapData(MapSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(mapDataSavePath, json);
    }

    

    public static MapSaveData GetMapFromSave()
    {
        if (!File.Exists(mapDataSavePath))
            return new MapSaveData();

        string json = File.ReadAllText(mapDataSavePath);
        return JsonUtility.FromJson<MapSaveData>(json);
    }

}


