public class GoCoveringTeammate : GuardAction 
{
    public override void Excute()
    {
        base.Excute();
        guard.moving.target = guard.heardAlert.alertTeammate.transform;
        // 调试用的
//         foreach(FOV2DEyes eye in guard.eyes)
//         {
//             eye.gameObject.SetActive(false);
//         }

        guard.moving.targetOffset = 
            UnityEngine.Quaternion.AngleAxis(UnityEngine.Random.Range(-10, 10), new UnityEngine.Vector3(0, 1, 0)) * 
            guard.moving.target.forward * 4.0f; 
        guard.moving.canMove = true;
        guard.moving.canSearch = true;
        guard.moving.SearchPath();
        UnityEngine.Debug.Log("GoCoveringTeammate");
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
