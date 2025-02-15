using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    [SerializeField] private Sound changeSound;
    [SerializeField] private Sound interactSound;
    private RectTransform arrow;
    private int currentOption;
    private void Awake()
    {
        arrow = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        currentOption = 0;
        ChangePosition(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            ChangePosition(-1);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            ChangePosition(1);

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.F))
            Interact();
    }
    private void ChangePosition(int _change)
    {
        currentOption += _change;

        if (_change != 0)
            SoundManager.instance.PlaySound(changeSound);

        if (currentOption < 0)
            currentOption = options.Length - 1;
        else if (currentOption > options.Length - 1)
            currentOption = 0;

        UpdateArrowPosition();
    }

    public void ChangePositionByIndex(int index)
    {
        if (index == currentOption) return;

        currentOption = index;
        SoundManager.instance.PlaySound(changeSound);
        UpdateArrowPosition();
    }

    private void UpdateArrowPosition()
    {
        RectTransform selectedOption = options[currentOption];

        Vector3 worldPosition = selectedOption.position;
        Vector3 localPosition = arrow.parent.InverseTransformPoint(worldPosition);

        float leftOffset = selectedOption.rect.width / 2 + arrow.rect.width / 2;

        arrow.localPosition = new Vector3(
            localPosition.x - leftOffset,
            localPosition.y,
            arrow.localPosition.z
     );
    }

    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);

        options[currentOption].GetComponent<Button>().onClick.Invoke();
    }
}
