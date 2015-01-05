public class Chase : GuardAction 
{
    public override void Excute()
    {
        base.Excute();        
        guard.moving.canMove = true;
        guard.moving.canSearch = true;
        guard.moving.SearchPath();
        if (guard.alertSound)
        {
            guard.alertSound.ChaseAlert();
        }
        UnityEngine.Debug.Log("chase");
    }

    public override void Stop()
    {
        base.Stop();
        if (guard.alertSound)
        {
            guard.alertSound.StopChaseAlert();
        }
        UnityEngine.Debug.Log("chase stop");
        guard.moving.canMove = false;
        guard.moving.canSearch = false;
    }
}
