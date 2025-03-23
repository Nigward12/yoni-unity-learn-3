
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointSaveData
{
    public List<string> checkpointKeys;
    public List<bool> checkpointValues;

    public CheckpointSaveData(Dictionary<string, bool> checkpointDict)
    {
        checkpointKeys = new List<string>(checkpointDict.Keys);
        checkpointValues = new List<bool>(checkpointDict.Values);
    }

    public Dictionary<string, bool> ToDictionary()
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        for (int i = 0; i < checkpointKeys.Count; i++)
        {
            dict[checkpointKeys[i]] = checkpointValues[i];
        }
        return dict;
    }
}
