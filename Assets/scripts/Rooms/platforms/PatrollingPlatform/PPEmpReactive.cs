using System.Collections.Generic;
using UnityEngine;

public class PPEmpReactive : PatrollingPlatform, EmpReactive
{
    [Header("Emp Reaction Settings")]
    public Collider2D empReactionZone;

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

    public void OnBecomingChosenEmpReactive()
    {

    }
}
