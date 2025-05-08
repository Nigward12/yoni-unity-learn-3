using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EMPBurst : ability
{
    private List<EmpReactive> empReactiveInProximity;

    private EmpReactive aimedEmpReactive;

    private Transform playerTransform;

    private MovementScript playerMovementScript;

    private void Start()
    {
        playerTransform = this.transform;
        playerMovementScript = playerTransform.GetComponent<MovementScript>();
        empReactiveInProximity = new List<EmpReactive>();
        aimedEmpReactive = null;
    }
    private void Update()
    {
        if (empReactiveInProximity.Count > 0)
        {
            SortEmpReactiveByDistance();
            aimedEmpReactive = ChooseAimedEmpReactive();
            if (aimedEmpReactive != null)
            {
                aimedEmpReactive.OnBecomingChosenEmpReactive();
                if (Input.GetKeyDown(abilityActivationKey))
                    ActivateAbility();
            }
        }
    }

    private void SortEmpReactiveByDistance()
    {
        empReactiveInProximity.Sort((a, b) =>
        {
            Transform ta = ((MonoBehaviour)a).transform;
            Transform tb = ((MonoBehaviour)b).transform;

            float da = Vector2.Distance(ta.position, playerTransform.position);
            float db = Vector2.Distance(tb.position, playerTransform.position);

            return da.CompareTo(db);
        });
    }

    private EmpReactive ChooseAimedEmpReactive()
    {
        // direction thing not working maybe, maybe its the glow thing
        bool playerFacingLeft = playerMovementScript.IsFacingLeft();

        foreach (var empReactive in empReactiveInProximity)
        {
            Transform tEmp = ((MonoBehaviour)empReactive).transform;
            Vector2 directionToTarget = tEmp.position - playerTransform.position;
            bool isToLeft = directionToTarget.x < 0;

            if (!empReactive.IsReacting() && 
                (playerFacingLeft && isToLeft) || (!playerFacingLeft && !isToLeft))
            {
                if (aimedEmpReactive != null && aimedEmpReactive != empReactive)
                    aimedEmpReactive.OnBecomingUnchosenEmpReactive();
                return empReactive;
            }
        }
        if (aimedEmpReactive != null)
            aimedEmpReactive.OnBecomingUnchosenEmpReactive();
        return null;
    }

    public void AddEmpReactiveInProximity(EmpReactive empReactive)
    {
        if (!empReactiveInProximity.Contains(empReactive))
            empReactiveInProximity.Add(empReactive);
    }

    public void RemoveEmpReactiveInProximity(EmpReactive empReactive)
    {
        if (empReactiveInProximity.Contains(empReactive))
        {
            empReactiveInProximity.Remove(empReactive);
            if (aimedEmpReactive == empReactive)
                aimedEmpReactive.OnBecomingUnchosenEmpReactive();
        }
    }

    public override void ActivateAbility()
    {
        // add playing the ability animation and particleSystems
        base.ActivateAbility();
        aimedEmpReactive.ReactToEmpBurst(PlayerManager.instance.playerData
            .abilitiesProperties["EmpBurst"]);
    }
}
