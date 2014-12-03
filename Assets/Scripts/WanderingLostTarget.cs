using UnityEngine;
using System.Collections;

public class WanderingLostTarget : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        Debug.Log("WanderingLostTarget");
        guard.anim.CrossFade("idle");
        Invoke("GoOnPatrol", 3.0f);
    }

    void GoOnPatrol()
    {        
        guard.backing.Excute();
    }
}
