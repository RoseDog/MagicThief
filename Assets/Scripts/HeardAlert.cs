public class HeardAlert : GuardAction 
{
    public Guard alertTeammate;
    public Cocos2dAction goCoverAction;
    public UnityEngine.Vector3 soundPosition;
	public void Heard(Guard teammate)
    {
        UnityEngine.Debug.Log(gameObject.name + ":HeardAlert");
        base.Excute();
        alertTeammate = teammate;
        guard.spriteSheet.Play("idle");
        guard.FaceTarget(alertTeammate.transform);

        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Suspicious);
                
        goCoverAction = guard.SleepThenCallFunction(50,() => GoCovering());

        System.String content = gameObject.name;
        content += " HeardAlert";
        Globals.record("testReplay", content);
    }

    public void HeardSound(UnityEngine.Vector3 soundPos)
    {
        // 如果不是第一次听到声音，那么只是更新一下声音的位置
        if(guard.currentAction == this)
        {
            soundPosition = soundPos;
            UnityEngine.Vector3 dir = soundPosition - transform.position;
            guard.FaceDir(dir);            
        }
        else if (guard.currentAction == guard.goCovering)
        {
            guard.GoTo(guard.heardAlert.soundPosition);
        }
        else
        {
            base.Excute();
            alertTeammate = null;
            guard.spriteSheet.Play("idle");
            guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Suspicious);
            goCoverAction = guard.SleepThenCallFunction(20, () => GoCovering());
        }
        
        System.String content = gameObject.name;
        content += " HeardSound";
        Globals.record("testReplay", content);    }

    public override void Stop()
    {
        if (goCoverAction != null)
        {
            guard.RemoveAction(ref goCoverAction);
        }        
        base.Stop();
    }

    void GoCovering()
    {
        goCoverAction = null;
        guard.goCovering.Excute();
    }
}
