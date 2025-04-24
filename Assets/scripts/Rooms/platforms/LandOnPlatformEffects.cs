using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class LandOnPlatformEffects : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> mustPlayEffects;

    [SerializeField] private List<ParticleSystem> randomizedPlayEffects;

    [SerializeField] private int minimunRandomEffectsPlayed;

    [SerializeField] private int maximumRandomEffectsPlayed = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayMustEffects();

        PlayRandomizedEffects(randomizedPlayEffects, minimunRandomEffectsPlayed, maximumRandomEffectsPlayed, 0.5f);
    }

    private void PlayRandomizedEffects(List<ParticleSystem> effects, int minimumCount, int maximumCount, float chance)
    {
        if (effects == null || effects.Count == 0)
            return;

        maximumCount = Mathf.Clamp(maximumCount, 0, effects.Count);
        minimumCount = Mathf.Clamp(minimumCount, 0, maximumCount);

        List<ParticleSystem> playedEffects = new List<ParticleSystem>();
        List<ParticleSystem> remaining = new List<ParticleSystem>();

        foreach (var effect in effects)
        {
            if (effect == null) continue;

            if (Random.value < chance)
            {
                playedEffects.Add(effect);
                if (playedEffects.Count >= maximumCount)
                    break;
            }
            else
            {
                remaining.Add(effect);
            }
        }

        int needed = minimumCount - playedEffects.Count;
        for (int i = 0; i < remaining.Count && playedEffects.Count < minimumCount; i++)
        {
            playedEffects.Add(remaining[i]);
        }

        foreach (var effect in playedEffects)
        {
            if (effect != null)
                effect.Play();
        }
    }

    private void PlayMustEffects()
    {
        foreach (var effect in mustPlayEffects)
        {
            if (effect != null)
                effect.Play();
        }
    }

}
