
public class TutorialLevelController : LevelController
{
    public UnityEngine.GameObject LevelTipBillboard;
    public override void Awake()
    {
        base.Awake();
        UnityEngine.GameObject billboard_prefab = UnityEngine.Resources.Load("Props/LevelTipBillboard") as UnityEngine.GameObject;
        LevelTipBillboard = UnityEngine.GameObject.Instantiate(billboard_prefab) as UnityEngine.GameObject;
        LevelTipBillboard.SetActive(false);
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Cell cellForLevelTip = Globals.map.allCorridorsAfterMazeCompleted[Globals.map.allCorridorsAfterMazeCompleted.Count - 2];
        if (Globals.map.LevelTipText != "")
        {
            LevelTipBillboard.SetActive(true);
            LevelTipBillboard.transform.position = cellForLevelTip.GetFloorPos();
            LevelTipBillboard.GetComponentInChildren<UnityEngine.UI.Text>().text = Globals.map.LevelTipText;
        }        
        if (Globals.magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
            UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;
        }
       
        Globals.magician.transform.position = Globals.map.entryOfMaze.GetFloorPos();
        Globals.map.RegistChallengerEvent();
    }

    public virtual void StealingBegin()
    {
        // 魔术师出场        
        Globals.magician.isInAir = false;        
        Globals.magician.gameObject.SetActive(true);

        // 相机跟随        
        Globals.cameraFollowMagician.enabled = true;
        Globals.cameraFollowMagician.beginFollow(Globals.magician.transform);
        Globals.transition.fadeColor.a = 1.0f;
        Globals.transition.BlackIn();
        // 允许输入
        Globals.EnableAllInput(true);
    }
}
