
using System.Collections.Generic;
using UnityEngine;

public interface EmpReactive
{
    public void ReactToEmpBurst(Dictionary<string, float> burstProperties);

    public bool IsReacting();

    public void OnBecomingChosenEmpReactive();
}
