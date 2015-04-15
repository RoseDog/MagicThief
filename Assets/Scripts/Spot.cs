using UnityEngine;
using System.Collections;

public class Spot : GuardAction 
{
    public Transform target;
    public Cocos2dAction chaseCountDown;
    public Cocos2dAction outVisionCountDown;

    public override void Awake()
    {
        base.Awake();
    }
    public void SpotMagician(GameObject magician, bool goChasing)
    {
        if (outVisionCountDown != null)
        {
            guard.RemoveAction(ref outVisionCountDown);
        }

        if (target == null)
        {
            target = magician.transform;
//             if (target == Globals.magician.transform && Globals.magician.hypnosis.data.IsInUse())
//             {
//                 guard.ShowTrickBtns();
//             }
            Excute();
            if (goChasing)
            {
                chaseCountDown = guard.SleepThenCallFunction(80, () => BeginChase());
            }
        }        

        if(goChasing && guard.currentAction == guard.wandering)
        {
            BeginChase();
        }

        System.String content = gameObject.name;
        content += " enter vision";
        Globals.record("testReplay", content);
    }

    public override void Excute()
    {
        base.Excute();
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Alert);

        guard.spriteSheet.Play("idle");
        guard.FaceTarget(target);        
        guard.moving.ClearPath();
        guard.moving.canMove = false;
        Debug.Log("spot");        
        if (guard.alertSound)
        {
            // 狗叫的时候视野会打开迷雾            
            guard.eye.SetLayer(10);
            guard.alertSound.SpotAlert();
        }

        System.String content = gameObject.name;
        content += " spot";
        Globals.record("testReplay", content);
    }

    public void Update()
    {
        if(guard.currentAction == this)
        {
            guard.eye.CastRays(target.position - guard.transform.position, false);
        }
    }

    public override void Stop()
    {        
        base.Stop();
        guard.moving.canMove = true;
        if (chaseCountDown != null)
        {
            guard.RemoveAction(ref chaseCountDown);            
        }
    }

    public void EnemyOutVision(int outVisionTime)
    {
        UnityEngine.Debug.Log("enemy out vision");
        
        if (!target.GetComponent<Actor>().inLight)
        {
            outVisionCountDown = guard.SleepThenCallFunction(outVisionTime, () => LostTarget());
        }

        System.String content = gameObject.name;
        content += " EnemyOutVision";
        Globals.record("testReplay", content);
    }

    public void LostTarget()
    {
        // rushAt最后会调用Wandering。防止重复调用。
        if (guard.chase == guard.currentAction)
        {
            guard.wandering.Excute();
        }

        System.String content = gameObject.name;
        content += " LostTarget";
        Globals.record("testReplay", content);
    }
    
    public void BeginChase()
    {
        chaseCountDown = null;        
        guard.moving.target = guard.spot.target;
        guard.chase.Excute();        
    }
}
