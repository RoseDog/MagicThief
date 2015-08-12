public class Chase : GuardAction 
{
    Actor actor;
    Magician mage;
    public override void Excute()
    {
        base.Excute();        
        guard.moving.canMove = true;
        guard.moving.canSearch = true;
        guard.moving.SearchPath(guard.OnPathComplete);
        if (guard.alertSound)
        {
            guard.alertSound.ChaseAlert();
        }
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Alert);
        UnityEngine.Debug.Log("chase");        
    }

    public override void FrameFunc()
    {
        base.FrameFunc();
        actor = guard.spot.target.GetComponent<Actor>();
        mage = actor as Magician;

        if (guard.atk != null &&
            UnityEngine.Vector3.Distance(guard.transform.position, guard.spot.target.position) < guard.data.atkShortestDistance)
        {
            guard.atk.Excute();
        }
        else if (guard.rushAt != null &&
            UnityEngine.Vector3.Distance(guard.transform.position, guard.spot.target.position) < guard.data.rushAtShortestDistance)
        {
            guard.rushAt.Excute();
        }
        else if (mage != null && guard.explode != null &&
            UnityEngine.Vector3.Distance(guard.transform.position, guard.spot.target.position) < guard.data.atkShortestDistance)
        {
            guard.explode.Excute();
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (guard.alertSound)
        {
            // 狗不叫的时候视野不能打开迷雾
            guard.eye.SetLayer(27);
            guard.alertSound.StopAlert();
        }
        UnityEngine.Debug.Log("chase stop");
        guard.moving.canMove = false;
        guard.moving.canSearch = false;
    }
}
