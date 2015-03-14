public class BeenPressDown : Action 
{
    TrickTimer timer;
    int duration = 100;
    Cocos2dAction pushAwayAction;
    public System.Collections.Generic.List<Guard> guardsPressingMage = new System.Collections.Generic.List<Guard>();
    public void PressedByGuard(Guard guard)
    {
        UnityEngine.Debug.Log("Pressed");

        System.String content = gameObject.name;
        content += " Pressed";
        Globals.record("testReplay", content);

        if (stopCall != null)
        {
            actor.RemoveAction(ref stopCall);
            actor.RemoveAction(ref jumpSequence);
        }
        if (guardsPressingMage.Count == 0)
        {
            base.Excute();
            actor.moving.canMove = false;
            actor.spriteSheet.Play("down_on_floor");

            timer = (UnityEngine.GameObject.Instantiate(Globals.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
            timer.BeginCountDown(gameObject, duration, new UnityEngine.Vector3(0, 1.5f, 0));

            pushAwayAction = actor.SleepThenCallFunction(duration, () => PushGuardsAway());
        }
        else
        {
            timer.AddFrameTime(duration);
            ((pushAwayAction as Sequence).actions[0] as SleepFor)._start_frame = UnityEngine.Time.frameCount;
            ((pushAwayAction as Sequence).actions[0] as SleepFor)._frameDuration = timer.GetLastFrameTime();
        }        
        guardsPressingMage.Add(guard);
        Globals.canvasForMagician.tricksInUsingPanel.gameObject.SetActive(false);
    }
    Cocos2dAction stopCall;
    Cocos2dAction jumpSequence;
    public void PushGuardsAway()
    {
        UnityEngine.Debug.Log("Push Guards Away");

        DestroyObject(timer.gameObject);
        timer = null;
        int jump_up_duration = actor.spriteSheet.GetAnimationLength("disguise");
        UnityEngine.Vector3 originPosition = actor.transform.position;
        UnityEngine.Vector3 to = originPosition + new UnityEngine.Vector3(0,0.5f,0);
        jumpSequence = new Sequence(
            new MoveTo(transform, to, jump_up_duration / 2),
            new MoveTo(transform, originPosition, jump_up_duration / 2));
        actor.AddAction(jumpSequence);

        // 没有挣扎起身的动作。先用易容的动作代替
        actor.spriteSheet.Play("disguise");
        stopCall = actor.SleepThenCallFunction(jump_up_duration, () => Stop());
        foreach (Guard guard in guardsPressingMage)
        {
            guard.rushAt.PushedAway();
        }
        guardsPressingMage.Clear();

        System.String content = gameObject.name;
        content += " PushGuardsAway";
        Globals.record("testReplay", content);
    }

    public override void Stop()
    {
        base.Stop();
        stopCall = null;
        jumpSequence = null;
        actor.moving.canMove = true;
        Globals.canvasForMagician.tricksInUsingPanel.gameObject.SetActive(true);
    }
}
