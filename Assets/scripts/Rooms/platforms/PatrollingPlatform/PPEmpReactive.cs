using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPEmpReactive : PatrollingPlatform, EmpReactive
{
    [Header("Emp Reaction Settings")]
    public Collider2D empReactionZone;
    public SpriteRenderer glowOnChosen;
    private bool chosen = false;
    // add glow on being chosen

    protected bool IsReactingToEmp = false;
    EMPBurst playerEmpBurstAbility;

    private void Start()
    {
        playerEmpBurstAbility = PlayerManager.instance.getCurrentPlayer().GetComponent<EMPBurst>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerEmpBurstAbility != null)
                playerEmpBurstAbility.AddEmpReactiveInProximity(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerEmpBurstAbility != null)
                playerEmpBurstAbility.RemoveEmpReactiveInProximity(this);
        }
    }
    public virtual void ReactToEmpBurst(Dictionary<string, float> burstProperties)
    {
        IsReactingToEmp = true;
    }

    protected void StopReactingToEmpBurst()
    {
        IsReactingToEmp = false;
    }
    public bool IsReacting()
    {
        return IsReactingToEmp;
    }

    public IEnumerator GlowTransitionFadeInOut(SpriteRenderer glowTranformRenderer, float fadeDuration, bool fadingIn)
    {
        float t = 0f;

        Color startColor = glowTranformRenderer.color;
        float startAlpha = startColor.a;
        float endAlpha = fadingIn ? 1f : 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeDuration);
            glowTranformRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        glowTranformRenderer.color = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
    }

    public void OnBecomingChosenEmpReactive()
    {
        if (!chosen)
        {
            chosen = true;
            StartCoroutine(GlowTransitionFadeInOut(glowOnChosen, 0.3f, true));
        }
    }

    public void OnBecomingUnchosenEmpReactive()
    {
        if (chosen)
        {
            chosen = false;
            StartCoroutine(GlowTransitionFadeInOut(glowOnChosen, 0.3f, false));
        }
    }
}
