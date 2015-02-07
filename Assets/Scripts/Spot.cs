using UnityEngine;
using System.Collections;

public class Spot : GuardAction 
{
    public Transform target;
    public void SpotMagician(GameObject magician, float outVisionTime, bool goChasing)
    {
        if (target != magician.transform)
        {
            target = magician.transform;
            if(target == Globals.magician.transform && Globals.magician.hypnosis.data.slotIdxInUsingPanel != -1)
            {
                guard.ShowTrickBtns();
            }
            Excute();
            if (goChasing)
            {
                Invoke("BeginChase", 1.0f);
            }            
        }

        if(goChasing && guard.currentAction == guard.wandering)
        {
            BeginChase();
        }
        // eyes.OnTriggerEnter会反复触发，OnTriggerStay和OnTriggerExit不会触发。所以才这样写
        if (this.IsInvoking("EnemyOutVision"))
        {
            this.CancelInvoke("EnemyOutVision");
        }

        if (!target.GetComponent<Actor>().inLight)
        {
            this.Invoke("EnemyOutVision", outVisionTime);
        }        
    }

    public override void Excute()
    {
        base.Excute();
        foreach(FOV2DEyes eye in guard.eyes)
        {
            eye.visionCone.status = FOV2DVisionCone.Status.Alert;
        }
                
        guard.anim.CrossFade("atkReady");
        guard.FaceTarget(target);
        Debug.Log("spot");        
        if (guard.alertSound)
        {
            guard.alertSound.SpotAlert();
        }
    }

    public override void Stop()
    {        
        base.Stop();
        if (guard.alertSound)
        {
            guard.alertSound.StopSpotAlert();
        }
        CancelInvoke("BeginChase");
    }

    public void EnemyOutVision()
    {
        UnityEngine.Debug.Log("enemy out vision");        
        
        // bug, repeat invoking EnemyOutVision
        if (guard.wandering != guard.currentAction)
        {
            guard.wandering.Excute();
        }
    }    

    void BeginChase()
    {
        guard.moving.target = guard.spot.target;
        guard.chase.Excute();        
    }
}
