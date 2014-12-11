
public class TutorialLevelController : LevelController
{
    public override void MazeFinished()
    {
        base.MazeFinished();        

        // 创建相机，不允许跟随
        UnityEngine.GameObject camera_follow_prefab = UnityEngine.Resources.Load("CameraFollowMagician") as UnityEngine.GameObject;
        UnityEngine.GameObject camera_follow = UnityEngine.GameObject.Instantiate(camera_follow_prefab) as UnityEngine.GameObject;
        Globals.map.SetRestrictToCamera(Globals.cameraFollowMagician);
    }

    public virtual void StealingBegin()
    {        
        Globals.magician.isInAir = false;

        // 相机跟随        
        Globals.cameraFollowMagician.enabled = true;
        Globals.cameraFollowMagician.beginFollow(Globals.magician.transform);
        Globals.transition.fadeColor.a = 1.0f;
        Globals.transition.BlackIn();
        // 允许输入
        Globals.EnableAllInput(true);
    }
}
