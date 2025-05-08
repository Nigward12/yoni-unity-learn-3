
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public float PlayerMaxHealth = 5;

    public Dictionary<string, Dictionary<string, float>> abilitiesProperties = 
        new Dictionary<string, Dictionary<string, float>>
        {
        {
            "EmpBurst", new Dictionary<string, float>
            {
                { "platformPauseTime", 5f }
            }
        }
    };

    //add items, level, stats and so on in here.....
}
