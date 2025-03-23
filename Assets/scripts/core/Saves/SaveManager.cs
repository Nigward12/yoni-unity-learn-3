using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static void SaveInCheckpoint(Checkpoint spawnCheckpoint)
    {

        PlayerManager.instance.SavePlayerData();

        PlayerPrefs.SetString(GameManager.SAVED_LEVEL_KEY, LoadingManager.instance.currentLevelData.levelName);

        PlayerPrefs.SetString(GameManager.SAVED_CHECKPOINT_KEY, spawnCheckpoint.name);

        PlayerPrefs.Save();
    }
}


