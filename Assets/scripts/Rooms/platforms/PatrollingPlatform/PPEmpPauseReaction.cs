using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PPEmpPauseReaction : PPEmpReactive
{
    [Header("glow")]
    public Transform centerGlowTransform;

    private Color glowTranformColor;
    private SpriteRenderer glowTranformRenderer;

    private void Awake()
    {
        glowTranformRenderer = centerGlowTransform.GetComponent<SpriteRenderer>();
        glowTranformColor = glowTranformRenderer.color;
    }

    public void PauseMovement(float pauseTime)
    {
        if (!isPaused)
            StartCoroutine(PauseRoutine(pauseTime));
    }

    private IEnumerator PauseRoutine(float time)
    {
        isPaused = true;
        yield return new WaitForSeconds(time);
        isPaused = false;
        StopReactingToEmpBurst();
    }


    public override void ReactToEmpBurst(Dictionary<string, float> burstProperties)
    {
        base.ReactToEmpBurst(burstProperties);
        PauseMovement(burstProperties["platformPauseTime"]);
        StartCoroutine(platformPauseGlowRoutine());
    }


    private IEnumerator platformPauseGlowRoutine()
    {
        StartCoroutine(TransitionFadeInOut(1f, true));

        while (isPaused)
            yield return null;

        StartCoroutine(TransitionFadeInOut(1f, false));
    }

    public IEnumerator TransitionFadeInOut(float fadeDuration, bool fadingIn)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (fadingIn)
                glowTranformRenderer.color = new Color(glowTranformColor.r, glowTranformColor.g, glowTranformColor.b,
                Mathf.Lerp(0, 1, t / fadeDuration));
            else
                glowTranformRenderer.color = new Color(glowTranformColor.r, glowTranformColor.g, glowTranformColor.b,
                Mathf.Lerp(1, 0, t / fadeDuration));
            yield return null;
        }
    }
}
