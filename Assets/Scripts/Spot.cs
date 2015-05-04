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
    public void SpotMagician(GameObject magician, bool goChasing, int spotDuration)
    {
        if (outVisionCountDown != null)
        {
            guard.RemoveAction(ref outVisionCountDown);
        }

        if (target != magician.transform)
        {            
            base.Excute();
            target = magician.transform;
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
                guard.alertSound.StartAlert();
            }

            System.String content = gameObject.name;
            content += " spot";
            Globals.record("testReplay", content);
            if (goChasing)
            {
                chaseCountDown = guard.SleepThenCallFunction(spotDuration, () => BeginChase());
            }
        }
        

        if(goChasing && guard.currentAction == guard.wandering)
        {
            BeginChase();
        }        
    }   

    public void Update()
    {
        if(guard.currentAction == this)
        {
            // 更新守卫的视野体，但是并不发送消息，不触发任何事件
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
        // 1.rushAt最后会调用Wandering。防止重复调用。
        // 2.丢失鸽子的情况
        if (guard.chase == guard.currentAction || guard.spot == guard.currentAction)
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
