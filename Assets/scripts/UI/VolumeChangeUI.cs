using UnityEngine;
using UnityEngine.UI;

public class VolumeChangeUI : MonoBehaviour
{
    [SerializeField] private string volumeName;
    [SerializeField] private string textIntro;
    private Text txt;

    private void Awake()
    {
        txt = GetComponent<Text>();
    }
    private void Update()
    {
        UpdateVolume();
    }
    private void UpdateVolume()
    {
        int volumeValue = (int) (PlayerPrefs.GetFloat(volumeName) * 100);
        txt.text = textIntro + volumeValue.ToString();
    }
}