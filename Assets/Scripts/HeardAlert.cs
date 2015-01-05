public class HeardAlert : GuardAction 
{
    public Guard alertTeammate;
	public void Heard(Guard teammate)
    {
        UnityEngine.Debug.Log(gameObject.name + ":HeardAlert");
        base.Excute();
        alertTeammate = teammate;
        guard.anim.CrossFade("idle");
        guard.FaceTarget(alertTeammate.transform);
        foreach (FOV2DEyes eye in guard.eyes)
        {
            eye.visionCone.status = FOV2DVisionCone.Status.Suspicious;
        }
        
        Invoke("GoCovering", 1.0f);
    }

    public override void Stop()
    {
        CancelInvoke("GoCovering");
        base.Stop();
    }

    void GoCovering()
    {
        guard.goCovering.Excute();
    }
}
