public class CatchByNet : Action
{
    TrickTimer timer;
    int duration = 300;
    Cocos2dAction breakNetAction;
    Cocos2dAction clearNetReduceAction;
    float netReduce = 1;
    int netTime;
    public override void Awake()
    {
        base.Awake();
        netTime = duration;
    }

    public void Catched(Spider spider)
    {
        UnityEngine.Debug.Log("CatchByNet");

        if (stopCall != null)
        {
            actor.RemoveAction(ref stopCall);
            actor.RemoveAction(ref jumpSequence);
        }

        if (clearNetReduceAction != null)
        {
            actor.RemoveAction(ref clearNetReduceAction);
        }

        //actor.ChangeLife(-spider.data.attackValue);

        netTime = (int)(duration / netReduce);
        netReduce += 0.25f;

        System.String content = gameObject.name;
        content += " CatchByNet netTime:" + netTime.ToString();
        Globals.record("testReplay", content);

        if (actor.IsLifeOver())
        {
            actor.hitted.Excute();
        }
        else
        {
            UnityEngine.GameObject soundPrefab = UnityEngine.Resources.Load("Misc/GunSound") as UnityEngine.GameObject;
            GuardAlertSound sound = (UnityEngine.GameObject.Instantiate(soundPrefab) as UnityEngine.GameObject).GetComponent<GuardAlertSound>();
            sound.transform.position = transform.position;
            sound.SetRadiusLimit(1500);
            sound.StartAlert();

            if (timer == null)
            {
                base.Excute();
                actor.moving.canMove = false;
                actor.spriteSheet.Play("catch_by_net");

                timer = (UnityEngine.GameObject.Instantiate(Globals.stealingController.magician.TrickTimerPrefab) as UnityEngine.GameObject).GetComponent<TrickTimer>();
                timer.BeginCountDown(gameObject, netTime, new UnityEngine.Vector3(0, 150f, 0));

                breakNetAction = actor.SleepThenCallFunction(netTime, () => BreakNet());
                foreach (Chest chest in Globals.maze.chests)
                {
                    if (chest.isMagicianNear)
                    {
                        chest.TouchOut(actor);
                    }
                }
            }
            else
            {
                timer.AddFrameTime(netTime);
                ((breakNetAction as Sequence).actions[0] as SleepFor)._start_frame = Globals.LevelController.frameCount;
                ((breakNetAction as Sequence).actions[0] as SleepFor)._frameDuration = timer.GetLastFrameTime();
            }
        }


        Globals.canvasForMagician.HideTricksPanel();
    }
    Cocos2dAction stopCall;
    public Cocos2dAction jumpSequence;
    public void BreakNet()
    {
        UnityEngine.Debug.Log("BreakNet");
        breakNetAction = null;
        Actor.to_be_remove.Add(timer);
        timer = null;
        int jump_up_duration = actor.spriteSheet.GetAnimationLengthWithSpeed("break_net");
        UnityEngine.Vector3 originPosition = actor.transform.position;
        UnityEngine.Vector3 to = originPosition + new UnityEngine.Vector3(0, 50f, 0);
        jumpSequence = new Sequence(
            new MoveTo(transform, to, jump_up_duration / 2),
            new MoveTo(transform, originPosition, jump_up_duration / 2));
        actor.AddAction(jumpSequence);

        
        actor.spriteSheet.Play("break_net");
        stopCall = actor.SleepThenCallFunction(jump_up_duration, () => Stop());


        UnityEngine.GameObject NetPrefab = UnityEngine.Resources.Load("Avatar/Net") as UnityEngine.GameObject;
        UnityEngine.GameObject Net = UnityEngine.GameObject.Instantiate(NetPrefab) as UnityEngine.GameObject;
        Net.transform.position = actor.transform.position;

        System.String content = gameObject.name;
        content += " BreakNet";
        Globals.record("testReplay", content);
    }

    public override void Stop()
    {
        base.Stop();
        if (breakNetAction != null)
        {
            actor.RemoveAction(ref breakNetAction);
        }        
        
        if (stopCall != null)
        {
            actor.RemoveAction(ref stopCall);
        }        

        if (timer != null)
        {
            Actor.to_be_remove.Add(timer);
            timer = null;
        }        
        
        if (jumpSequence != null)
        {
            actor.RemoveAction(ref jumpSequence);
        }
        
        if (!actor.IsLifeOver())
        {
            // 教程中的盗贼被扑倒一次就不会逃跑了
            actor.moving.canMove = true;
            Globals.canvasForMagician.ShowTricksPanel();            
        }


        clearNetReduceAction = actor.SleepThenCallFunction(300, () => ClearNetReduce());
    }

    public void ClearNetReduce()
    {
        netTime = duration;
        netReduce = 1;
        clearNetReduceAction = null;
    }
}

