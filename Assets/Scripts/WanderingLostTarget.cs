using UnityEngine;
using System.Collections;

public class WanderingLostTarget : GuardAction 
{
    Cocos2dAction call;
    Cocos2dAction eyeWandering;
    public override void Excute()
    {
        base.Excute();
        if (guard.alertSound != null)
        {
            guard.alertSound.StopAlert();
        }
        Debug.Log("WanderingLostTarget:" + UnityEngine.Time.frameCount.ToString());
        if (guard.heardAlert != null)
        {
            guard.heardAlert.alertTeammate = null;
        }
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Suspicious);
        
        if (guard.spriteSheet.HasAnimation("wander"))
        {
            guard.spriteSheet.Play("wander");
        }
        else
        {
            guard.spriteSheet.Play("idle");
        }
        guard.spot.target = null;
        guard.moving.target = null;
        guard.moving.canMove = false;

        if (eyeWandering == null)
        {
            float wandering_angle = 60;
            eyeWandering = new Sequence(
                new RotateEye(guard.eye, new Vector3(0, 0, wandering_angle), 20),
                new SleepFor(60),
                new RepeatForever(                
                new RotateEye(guard.eye, new Vector3(0, 0, -wandering_angle*2), 40),
                new SleepFor(60),
                new RotateEye(guard.eye, new Vector3(0, 0, wandering_angle*2), 40),
                new SleepFor(60)));
            guard.AddAction(eyeWandering);
        }
        
        if (call == null)
        {
            call = guard.SleepThenCallFunction(250, () => GoOnPatrol());            
        }
        else
        {
            ((call as Sequence).actions[0] as SleepFor)._start_frame = UnityEngine.Time.frameCount;
        }        
    }

    public override void Stop()
    {
        guard.moving.canMove = true;
        guard.RemoveAction(ref call);
        guard.RemoveAction(ref eyeWandering);
        Debug.Log("stop wandering:" + UnityEngine.Time.frameCount.ToString());
        base.Stop();
    }

    void GoOnPatrol()
    {        
        guard.backing.Excute();
    }
}
