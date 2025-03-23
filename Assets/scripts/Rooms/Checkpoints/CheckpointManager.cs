using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance { get; private set; }

    private Dictionary<string, bool> checkpointStates = new Dictionary<string, bool>();

    private string saveFilePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "checkpointData.json");
        LoadCheckpointData();
    }

    public bool IsCheckpointDiscovered(string checkpointKey)
    {
        return checkpointStates.ContainsKey(checkpointKey) && checkpointStates[checkpointKey];
    }

    public void DiscoverCheckpoint(string checkpointKey)
    {
        if (!checkpointStates.ContainsKey(checkpointKey))
            checkpointStates.Add(checkpointKey, true);
        else
            checkpointStates[checkpointKey] = true;

        SaveCheckpointData();
    }

    private void SaveCheckpointData()
    {
        string json = JsonUtility.ToJson(new CheckpointSaveData(checkpointStates));
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadCheckpointData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            CheckpointSaveData loadedData = JsonUtility.FromJson<CheckpointSaveData>(json);
            checkpointStates = loadedData.ToDictionary();
        }
    }
}

