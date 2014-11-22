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
        guard.moving.GetSeeker().StartPath(guard.moving.GetFeetPosition(), guard.birthCell.GetFloorPos());
    }

    public override void Stop()
    {
        base.Stop();
        guard.moving.canMove = false;        
    }
}
