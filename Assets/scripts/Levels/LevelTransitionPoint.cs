using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionPoint : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private float minTransitionTime;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StartCoroutine(TransitionToScene());
    }

    private IEnumerator TransitionToScene()
    {
        yield return StartCoroutine(UiManager.instance.TransitionFadeIn(minTransitionTime/2));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextLevelName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return null;
        yield return StartCoroutine(UiManager.instance.TransitionFadeOut(minTransitionTime / 2)); 
    }
}
