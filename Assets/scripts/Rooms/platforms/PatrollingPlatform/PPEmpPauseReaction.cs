using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PPEmpPauseReaction : PPEmpReactive
{
    public SpriteRenderer glowOnEmpReaction;


    public void PauseMovement(float pauseTime)
    {
        StartCoroutine(PauseForSeconds(pauseTime));                 
        StartCoroutine(EmpPauseEndWatcher(pauseTime));              
    }

    public override void ReactToEmpBurst(Dictionary<string, float> burstProperties)
    {
        base.ReactToEmpBurst(burstProperties);
        PauseMovement(burstProperties["platformPauseTime"]);
        StartCoroutine(platformPauseGlowRoutine());
    }

    private IEnumerator EmpPauseEndWatcher(float duration)
    {
        yield return new WaitForSeconds(duration);   
        StopReactingToEmpBurst();                    
    }

    private IEnumerator platformPauseGlowRoutine()
    {
        StartCoroutine(GlowTransitionFadeInOut(glowOnEmpReaction, 0.5f, true));

        while (isPaused)
            yield return null;

        StartCoroutine(GlowTransitionFadeInOut(glowOnEmpReaction, 0.5f, false));
    }

}
