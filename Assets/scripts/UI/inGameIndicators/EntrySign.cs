using TMPro;
using UnityEngine;
using System.Collections;

public class EntrySign : MonoBehaviour
{
    [SerializeField] private SpriteRenderer signTop;
    [SerializeField] private SpriteRenderer signBottom;
    [SerializeField] private SpriteRenderer signArrow;
    [SerializeField] private TextMeshPro signText;
    [SerializeField] private LevelData signEntryLevelData;
    private Vector3 arrowTopLimit;
    private Vector3 arrowBottomLimit;

    private bool isSignVisible = false;
    private Coroutine arrowHopRoutine;

    private void Awake()
    {
        signTop.color = new Color(signTop.color.r, signTop.color.g, signTop.color.b, 0);
        signBottom.color = new Color(signBottom.color.r, signBottom.color.g, signBottom.color.b, 0);
        signArrow.color = new Color(signArrow.color.r, signArrow.color.g, signArrow.color.b, 0);
        signText.color = new Color(signText.color.r, signText.color.g, signText.color.b, 0);
        arrowTopLimit = signArrow.transform.position;

        arrowBottomLimit = signArrow.transform.position;
        arrowBottomLimit.y = arrowBottomLimit.y - 0.2f;
    }

    private void Update()
    {
        if (isSignVisible && Input.GetKeyDown(KeyCode.W))
        {
            EnterLevel();
            this.enabled = false;
        }
    }

    private void EnterLevel()
    {
        CinemachineCameraManager.instance.TargetSwapGeneric(transform);
        LoadingManager.instance.TransitionToScene(signEntryLevelData, 1, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerFeet")
            ShowSign();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerFeet")
            StopShowingSign();
    }

    private void ShowSign()
    {
        isSignVisible = true;

        StartCoroutine(LerpSpriteAlpha(signTop, true));
        StartCoroutine(LerpSpriteAlpha(signBottom, true));
        StartCoroutine(LerpTextAlpha(signText, true));
        StartCoroutine(LerpSpriteAlpha(signArrow, true));

        arrowHopRoutine = StartCoroutine(HopArrowUpAndDown());
    }

    private void StopShowingSign()
    {
        isSignVisible = false;

        StartCoroutine(LerpSpriteAlpha(signTop, false));
        StartCoroutine(LerpSpriteAlpha(signBottom, false));
        StartCoroutine(LerpTextAlpha(signText, false));
        StartCoroutine(LerpSpriteAlpha(signArrow, false));

        if (arrowHopRoutine != null)
            StopCoroutine(arrowHopRoutine);
    }

    private IEnumerator LerpSpriteAlpha(SpriteRenderer signPart, bool fadeIn)
    {
        float t = 0f;
        Color color = signPart.color;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / 0.3f);
            color.a = alpha;
            signPart.color = color;
            yield return null;
        }
        color.a = endAlpha;
        signPart.color = color;
    }

    private IEnumerator LerpTextAlpha(TextMeshPro text, bool fadeIn)
    {
        float t = 0f;
        Color color = text.color;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / 0.3f);
            color.a = alpha;
            text.color = color;
            yield return null;
        }

        color.a = endAlpha;
        text.color = color;
    }

    private IEnumerator HopArrowUpAndDown()
    {
        float duration = 0.5f;

        while (isSignVisible)
        {
            yield return StartCoroutine(MoveArrow(signArrow.transform, arrowBottomLimit, duration));
            yield return StartCoroutine(MoveArrow(signArrow.transform, arrowTopLimit, duration));
        }
    }

    private IEnumerator MoveArrow(Transform arrow, Vector3 targetPos, float duration)
    {
        float t = 0f;
        float startY = arrow.position.y;
        float endY = targetPos.y;


        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            float newY = Mathf.Lerp(startY, endY, progress);
            arrow.position = new Vector3(arrow.position.x, newY, arrow.position.z);
            yield return null;
        }

        arrow.position = new Vector3(arrow.position.x, endY, arrow.position.z);
    }
}
