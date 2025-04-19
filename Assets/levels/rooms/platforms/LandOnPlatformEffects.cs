using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class LandOnPlatformEffects : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> mustPlayEffects;

    [SerializeField] private List<ParticleSystem> randomizedPlayEffects;

    [SerializeField] private int minimunRandomEffectsPlayed;

}
