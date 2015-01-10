public class Disguise : Action 
{
    float duration = 5.0f;
    float originSpeed;
    UnityEngine.GameObject FakeGuard_prefab;
    UnityEngine.GameObject FakeGuard;
    UnityEngine.GameObject TrickTimerPrefab;
    UnityEngine.GameObject TrickTimer;    
    public override void Awake()
    {
        base.Awake();
        FakeGuard_prefab = UnityEngine.Resources.Load("Avatar/FakeGuard") as UnityEngine.GameObject;
        TrickTimerPrefab = UnityEngine.Resources.Load("UI/FakeGuardTimer") as UnityEngine.GameObject;
    }
    public override void Excute()
    {
        if (actor.currentAction == this)
        {
            TrickTimer.GetComponent<TrickTimer>().AddTime(duration);
            CancelInvoke("Stop");
        }
        else
        {
            base.Excute();
            actor.Visible(false);

            FakeGuard = UnityEngine.GameObject.Instantiate(FakeGuard_prefab) as UnityEngine.GameObject;
            FakeGuard.transform.position = transform.position;
            FakeGuard.transform.rotation = transform.rotation;
            FakeGuard.transform.parent = transform;
            actor.anim = FakeGuard.animation;

            TrickTimer = UnityEngine.GameObject.Instantiate(TrickTimerPrefab) as UnityEngine.GameObject;
            TrickTimer.GetComponent<TrickTimer>().BeginCountDown(FakeGuard, duration, new UnityEngine.Vector3(0, 2.8f, 0));

            originSpeed = actor.moving.speed;
            actor.moving.speed = 0.04f;
            gameObject.layer = 0;
        }
        
        Invoke("Stop", duration);
    }

    public override void Stop()
    {
        base.Stop();        
        DestroyObject(FakeGuard);
        DestroyObject(TrickTimer);        
        actor.Visible(true);
        actor.anim = animation;
        actor.moving.speed = originSpeed;
        gameObject.layer = 11;
    }
}
