using System.Collections;

public class BackToBirthCell : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        UnityEngine.Debug.Log("BackToBirthCell");
        foreach (FOV2DEyes eye in guard.eyes)
        {
            eye.visionCone.status = FOV2DVisionCone.Status.Idle;
        }                
        guard.spot.target = null;
        guard.moving.target = null;
        guard.moving.canMove = true;
        guard.moving.canSearch = false;
        guard.MoveTo(Globals.GetPathNodePos(guard.birthNode) + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f),guard.moving.PathComplete);
    }

    public override void Stop()
    {
        base.Stop();
        guard.moving.canMove = false;        
    }
}
