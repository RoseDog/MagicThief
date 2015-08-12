using System.Collections;

public class BackToBirthCell : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        UnityEngine.Debug.Log("BackToBirthCell:" + Globals.LevelController.frameCount.ToString());
        guard.eye.SetVisionStatus(FOV2DVisionCone.Status.Idle);        
        guard.moving.canMove = true;
        guard.moving.canSearch = false;
        guard.GoTo(Globals.GetPathNodePos(guard.birthNode) + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));
    }

    public override void Stop()
    {
        base.Stop();
        guard.moving.canMove = false;        
    }
}
