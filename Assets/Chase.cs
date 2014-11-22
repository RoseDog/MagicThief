using System.Collections;

public class Chase : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        guard.moving.target = guard.spot.target;
        guard.moving.canMove = true;
        guard.moving.canSearch = true;
        guard.moving.SearchPath();
        UnityEngine.Debug.Log("chase");
    }

    public override void Stop()
    {
        base.Stop();
        UnityEngine.Debug.Log("chase stop");
        guard.moving.canMove = false;
        guard.moving.canSearch = false;
    }
}
