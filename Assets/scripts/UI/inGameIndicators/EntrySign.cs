using TMPro;
using UnityEngine;
using System.Collections;

public class EntrySign : MonoBehaviour
{
    [SerializeField] private SpriteRenderer signTop;
    [SerializeField] private SpriteRenderer signBottom;
    [SerializeField] private SpriteRenderer signArrow;
    [SerializeField] private TextMeshPro signText;

    private Vector3 signTextOriginalPos;

    private void Awake()
    {
        signTop.color = new Color(signTop.color.r, signTop.color.g, signTop.color.b, 0);
        signBottom.color = new Color(signBottom.color.r, signBottom.color.g, signBottom.color.b, 0);
        signArrow.color = new Color(signArrow.color.r, signArrow.color.g, signArrow.color.b, 0);
        signText.color = new Color(signText.color.r, signText.color.g, signText.color.b, 0);

        signTextOriginalPos = signText.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && PlayerManager.instance.getCurrentPlayer()
            .GetComponent<PlayerBasicMovement>().isGrounded)
            ShowSign();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            StopShowingSign();
    }

    private void ShowSign()
    {
        StartCoroutine(LerfSpriteAlpha(signTop));
        StartCoroutine(LerfSpriteAlpha(signBottom));
        StartCoroutine(LerpTextAlpha(signText));
        StartCoroutine(LerfSpriteAlpha(signTop));
    }

    private void StopShowingSign()
    {
        
    }

    private IEnumerator LerfSpriteAlpha(SpriteRenderer signPart)
    {
        // add lerping out and not only in
        float t = 0;
        Color color = signPart.color;

        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0, 1, t / 0.3f);
            signPart.color = color;
            yield return null;
        }
    }

    private IEnumerator LerpTextAlpha(TextMeshPro text)
    {
        float t = 0;
        Color color = text.color;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / 0.3f);
            text.color = color;
            yield return null;
        }
    }
}
