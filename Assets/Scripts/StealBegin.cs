public class StealBegin : UnityEngine.MonoBehaviour
{
    bool began = false;
	void Update () 
    {
        if (!began && Globals.map.entryOfMaze != null)
        {
            began = true;
            Globals.map.UnRegistDefenderEvent();
            Globals.map.RegistChallengerEvent();
            UnityEngine.CanvasRenderer[] canvases = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.CanvasRenderer>();
            foreach (UnityEngine.CanvasRenderer canvas in canvases)
            {
//                 if (canvas.gameObject == textOnBtn.gameObject)
//                 {
//                     textOnBtn.text = "结束游戏";
//                 }
//                 else if (canvas.gameObject != this.gameObject)
//                 {
//                     canvas.gameObject.SetActive(false);
//                 }
            }

            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
            UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;

            Cell magician_birth = Globals.map.entryOfMaze;
            magician.name = "Mage_Girl";
            magician.transform.position = magician_birth.GetFloorPos();            

            // 相机跟随
            UnityEngine.GameObject camera_follow_prefab = UnityEngine.Resources.Load("CameraFollowMagician") as UnityEngine.GameObject;
            UnityEngine.GameObject camera_follow = UnityEngine.GameObject.Instantiate(camera_follow_prefab) as UnityEngine.GameObject;
            Globals.cameraFollowMagician.beginFollow(magician.transform);
        }               
	}
}
