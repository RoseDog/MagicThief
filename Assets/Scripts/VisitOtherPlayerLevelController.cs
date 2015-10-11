using UnityEngine;
using System.Collections;

public class VisitOtherPlayerLevelController : LevelController 
{
    public UnityEngine.GameObject canvasForVisit;
    UnityEngine.UI.Button LeaveBtn;
    UnityEngine.UI.RawImage NameOfOtherPlayerBg;
    UnityEngine.UI.Text NameOfOtherPlayer;
    public override void Awake()
    {
        base.Awake();
        canvasForVisit = UnityEngine.GameObject.Find("CanvasForVisit") as UnityEngine.GameObject;
        mainCanvas = canvasForVisit.GetComponent<UnityEngine.Canvas>();
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForVisit, "LeaveBtn");
        LeaveBtn.onClick.AddListener(() => ReturnCity());
        LeaveBtn.gameObject.SetActive(false);
        NameOfOtherPlayerBg = Globals.getChildGameObject<UnityEngine.UI.RawImage>(canvasForVisit, "NameOfOtherPlayer");
        NameOfOtherPlayer = Globals.getChildGameObject<UnityEngine.UI.Text>(NameOfOtherPlayerBg.gameObject, "Text");
        NameOfOtherPlayerBg.gameObject.SetActive(false);
    }

    
    public override void BeforeGenerateMaze()
    {
        Globals.iniFileName = "MyMaze_" + Globals.visitPlayer.currentMazeLevel.ToString();
        Globals.ReadMazeIniFile(Globals.iniFileName, Globals.visitPlayer.currentMazeRandSeedCache);
        NameOfOtherPlayerBg.gameObject.SetActive(true);
        Globals.canvasForMagician.SetCashVisible(false);
        Globals.canvasForMagician.SetRoseVisible(false);
        Globals.languageTable.SetText(NameOfOtherPlayer, "other_player_maze_name", new System.String[] { Globals.visitPlayer.name, Globals.visitPlayer.currentMazeLevel.ToString() });
        base.BeforeGenerateMaze();
    }


    public override IniFile GetGuardsIniFile()
    {
        IniFile ini = new IniFile();
        ini.loadFromText(Globals.visitPlayer.summonedGuardsStr);
        return ini;        
    }    

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.ReadMazeIniFile(Globals.iniFileName, Globals.visitPlayer.currentMazeRandSeedCache);        

        Globals.maze.RegistChallengerEvent();

        Globals.cameraFollowMagician.Reset();
        Globals.EnableAllInput(true);
        LeaveBtn.gameObject.SetActive(true);

        SyncWithChestData(Globals.visitPlayer);
    }

    public override void FrameFunc()
    {
        base.FrameFunc();        
    }

    public override void ClickOnMap(UnityEngine.Vector2 finger_pos)
    {
        base.ClickOnMap(finger_pos);

        int mask = 1 << 10 | 1 << 27;
        Guard guard = Globals.FingerRayToObj<Guard>(
            Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

        mask = 1 << 14;
        Chest chest = Globals.FingerRayToObj<Chest>(Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>(), mask, finger_pos);

        if (guard != null)
        {
            guard.GuardInfoBtnClicked();
        }
        else if (chest != null)
        {
            chest.UpgradeBtnClicked();
        }               
    }

    public void ReturnCity()
    {
        canvasForVisit.gameObject.SetActive(false);
        Globals.cameraFollowMagician.CloseMinimap();
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public override void OnDestroy()
    {
        Globals.visitPlayer = null;
        base.OnDestroy();
    }
}
