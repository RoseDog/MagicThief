using UnityEngine;
using System.Collections;

public class Spot : GuardAction 
{
    public Transform target;
    public Cocos2dAction chaseCountDown;
    public Cocos2dAction outVisionCountDown;
    public Cocos2dAction turnToAtkReadyAct;
    public UnityEngine.AudioClip alert;
    public override void Awake()
    {
        base.Awake();        
        actor.spriteSheet.AddAnimationEvent("spot", -1, () => SpotTurnToAtkReady());
    }

    public void SpotTurnToAtkReady()
    {
        if (turnToAtkReadyAct == null)
        {
            turnToAtkReadyAct = guard.SleepThenCallFunction(15, () => TurnToAtkReady());        
        }        
    }

    void TurnToAtkReady()
    {
        guard.spriteSheet.Play("atkReady");
    }

    public void SpotMagician(GameObject newTar, bool goChasing, int spotDuration)
    {
        if (outVisionCountDown != null)
        {
            guard.RemoveAction(ref outVisionCountDown);
        }
        
        if (guard.CheckIfChangeTarget(newTar))
        {            
            base.Excute();
            
            if(target == null)
            {
                guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Alert);    
                guard.spriteSheet.Play("spot");
                newTar.GetComponent<Actor>().SpotByEnemy(guard);
            }
            else
            {
                target.GetComponent<Actor>().EnemyStopChasing(guard);
            }

            target = newTar.transform;
        
            guard.FaceTarget(target);
            guard.moving.ClearPath();
            guard.moving.canMove = false;
            Debug.Log("spot");
            if (guard.alertSound)
            {
                // 狗叫的时候视野会打开迷雾            
                guard.eye.SetLayer(10);
                guard.alertSound.StartAlert(true);
                guard.audioSource.clip = alert;
                guard.audioSource.loop = true;
                guard.audioSource.Play();
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

    public override void FrameFunc()
    {
        base.FrameFunc();
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

        if (turnToAtkReadyAct != null)
        {
            guard.RemoveAction(ref turnToAtkReadyAct);
        }

        guard.audioSource.Stop();
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
