using UnityEngine;
using System.Collections;

public class WanderingLostTarget : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        Debug.Log("WanderingLostTarget");
        if (guard.heardAlert != null)
        {
            guard.heardAlert.alertTeammate = null;
        }
        guard.anim.CrossFade("idle");
        guard.HideBtns();
        Invoke("GoOnPatrol", 3.0f);
    }

    public override void Stop()
    {
        CancelInvoke("GoOnPatrol");
        base.Stop();
    }

    void GoOnPatrol()
    {        
        guard.backing.Excute();
    }
}
