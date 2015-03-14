public class GoCoveringTeammate : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        guard.moving.canMove = true;
        if (guard.heardAlert.alertTeammate != null)
        {
            guard.moving.target = guard.heardAlert.alertTeammate.transform;
            guard.moving.targetOffset =
            UnityEngine.Quaternion.AngleAxis(UnityEngine.Random.Range(-10, 10), new UnityEngine.Vector3(0, 1, 0)) *
            guard.moving.target.forward * 4.0f;            
            guard.moving.canSearch = true;
            guard.moving.SearchPath(guard.OnPathComplete);
            UnityEngine.Debug.Log("GoCoveringTeammate");

            System.String content = gameObject.name;
            content += " GoCoveringTeammate";
            Globals.record("testReplay", content);
        }
        else
        {
            guard.moving.canSearch = false;
            guard.GoTo(guard.heardAlert.soundPosition);
        }                
    }

    public override void Stop()
    {
        base.Stop();
        UnityEngine.Debug.Log("GoCovering stop");
        guard.moving.targetOffset = UnityEngine.Vector3.zero; 
        guard.moving.canMove = false;
        guard.moving.canSearch = false;
    }
}
