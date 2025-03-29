using UnityEngine;

public class PlantSway : MonoBehaviour
{
    public float swayAmount = 5f;
    public float swaySpeed = 2f;

    private float initialZRotation;

    void Awake()
    {
        initialZRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float newZRotation = initialZRotation + sway;

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, newZRotation);
    }
}
