public class BeenHypnosised : GuardAction
{
    UnityEngine.GameObject TrickTimerPrefab;
    UnityEngine.GameObject TrickTimer;
    public override void Awake()
    {
        base.Awake();
        TrickTimerPrefab = UnityEngine.Resources.Load("UI/FakeGuardTimer") as UnityEngine.GameObject;
    }

    public void GoToSleep(float duration)
    {
        base.Excute();
        actor.anim.Play("BeenHypnosised");
        actor.moving.canMove = false;
        guard.HideBtns();
        guard.spot.target = null;
        guard.spot.CancelInvoke("EnemyOutVision");
        guard.EnableEyes(false);
        guard.gameObject.layer = 0;

        TrickTimer = UnityEngine.GameObject.Instantiate(TrickTimerPrefab) as UnityEngine.GameObject;
        TrickTimer.GetComponent<TrickTimer>().BeginCountDown(gameObject, duration, new UnityEngine.Vector3(0, 2.8f, 0));

        Invoke("Stop", duration);
    }

    public override void Stop()
    {
        base.Stop();
        DestroyObject(TrickTimer);
        guard.wakeFromHypnosised.Excute();
    }
}
