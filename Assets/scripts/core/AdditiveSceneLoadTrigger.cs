using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private SceneField[] _scenesToLoad;
    [SerializeField] private float _activationOffset = 10f;

    private GameObject _player;
    private Dictionary<string, AsyncOperation> _sceneLoadOperations = new();

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        foreach (SceneField sceneField in _scenesToLoad)
            if (!IsSceneLoaded(sceneField.SceneName))
                StartCoroutine(LoadSceneGradually(sceneField));

        UnloadAllScenesExcept(_scenesToLoad);
    }

    private IEnumerator LoadSceneGradually(SceneField sceneField)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneField, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;
        _sceneLoadOperations[sceneField.SceneName] = loadOp;

        // Wait until the scene is mostly loaded
        while (loadOp.progress < 0.9f)
        {
            yield return null;
        }

        // Wait for player to get close enough to activate
        while (!ShouldActivateScene())
        {
            yield return null;
        }

        // Now allow activation
        loadOp.allowSceneActivation = true;
    }

    private bool ShouldActivateScene()
    {
        if (_player == null) return false;

        float playerX = _player.transform.position.x;
        float triggerX = transform.position.x;

        return playerX > triggerX + _activationOffset;
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }
        return false;
    }

    private void UnloadAllScenesExcept(SceneField[] keepScenes)
    {
        HashSet<string> keepSceneNames = new();
        foreach (var sf in keepScenes)
            keepSceneNames.Add(sf.SceneName);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (!keepSceneNames.Contains(loadedScene.name))
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }
    }
}