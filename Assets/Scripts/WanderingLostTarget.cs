using UnityEngine;
using System.Collections;

public class WanderingLostTarget : GuardAction 
{
    Cocos2dAction call;
    Cocos2dAction eyeWandering;

    public UnityEngine.AudioClip wandering;
    public override void Excute()
    {
        base.Excute();
        if (guard.alertSound != null)
        {
            guard.alertSound.StopAlert();
        }
        Debug.Log("WanderingLostTarget:" + Globals.LevelController.frameCount.ToString());
        if (guard.heardAlert != null)
        {
            guard.heardAlert.alertTeammate = null;
        }
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Suspicious);        

        if (guard.spriteSheet.HasAnimation("suspicious"))
        {
            guard.spriteSheet.Play("suspicious");
        }
        else
        {
            guard.spriteSheet.Play("wander");
        }
        guard.spot.target = null;
        guard.moving.target = null;
        guard.moving.canMove = false;

        if (eyeWandering == null)
        {
            float wandering_angle = 60;
            eyeWandering = new Sequence(
                new RotateEye(guard.eye, new Vector3(0, 0, wandering_angle), 10),
                new FunctionCall(() => Sound()),
                new SleepFor(40),                
                new RepeatForever(                
                new RotateEye(guard.eye, new Vector3(0, 0, -wandering_angle*2), 25),
                new SleepFor(40),
                new RotateEye(guard.eye, new Vector3(0, 0, wandering_angle*2), 25),
                new SleepFor(40)));
            guard.AddAction(eyeWandering);
        }
        
        if (call == null)
        {
            call = guard.SleepThenCallFunction(150, () => GoOnPatrol());            
        }
        else
        {
            ((call as Sequence).actions[0] as SleepFor)._start_frame = Globals.LevelController.frameCount;
        }
        Globals.stealingController.magician.EnemyStopChasing(guard);
    }

    void Sound()
    {
        guard.audioSource.PlayOneShot(wandering);
    }

    public override void Stop()
    {
        guard.moving.canMove = true;
        guard.RemoveAction(ref call);
        guard.RemoveAction(ref eyeWandering);
        Debug.Log("stop wandering:" + Globals.LevelController.frameCount.ToString());
        base.Stop();
    }

    void GoOnPatrol()
    {        
        guard.backing.Excute();
    }
}
