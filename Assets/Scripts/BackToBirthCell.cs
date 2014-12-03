using System.Collections;

public class BackToBirthCell : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        UnityEngine.Debug.Log("BackToBirthCell");
        guard.spot.target = null;
        guard.moving.target = null;
        guard.moving.canMove = true;
        guard.moving.canSearch = false;
        guard.moving.GetSeeker().StartPath(guard.moving.GetFeetPosition(),
            Globals.GetPathNodePos(guard.birthNode) + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));
    }

    public override void Stop()
    {
        base.Stop();
        guard.moving.canMove = false;        
    }
}
