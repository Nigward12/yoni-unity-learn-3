using UnityEngine;

public class LevelTransitionPoint : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            LoadingManager.instance.LoadScene(nextLevelName);
    }
}
