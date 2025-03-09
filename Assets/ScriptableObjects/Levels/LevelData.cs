using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
[Serializable]
public class TransitionInfo
{
    public LevelData fromSceneData;  
    public string camTargetName;
    public string spawnPoint;
    public GameObject camActiveAfterTransitionPrefab;
}

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;

    public Sound levelMusic;

    [SerializeField] private List<TransitionInfo> transitionsList = new List<TransitionInfo>();

    private Dictionary<string, TransitionInfo> transitionInfoByFromScene;

    public Dictionary<string, TransitionInfo> TransitionInfoByFromScene
    {
        get
        {
            if (transitionInfoByFromScene == null)
            {
                transitionInfoByFromScene = new Dictionary<string, TransitionInfo>();

                foreach (var transition in transitionsList)
                {
                    if (transition.fromSceneData != null && !transitionInfoByFromScene.ContainsKey(transition.fromSceneData.levelName))
                    {
                        transitionInfoByFromScene.Add(transition.fromSceneData.levelName, transition);
                    }
                }
            }
            return transitionInfoByFromScene;
        }
    }
}
