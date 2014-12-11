public class Tutorial_Level_1_Controller : TutorialLevelController 
{
    int restartInSeconds = 8;
    public UnityEngine.GameObject canvasForStealingBegin;
    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.EnableAllInput(true);
        Globals.map.RegistChallengerEvent();
        Globals.canvasForMagician.SetLifeVisible(true);
        Globals.canvasForMagician.tutorialText.gameObject.SetActive(true);        
        Globals.canvasForMagician.tutorialText.text = "小心这些守卫";
        Globals.canvasForMagician.RestartText.gameObject.SetActive(false);
        Globals.canvasForMagician.lifeNumber.Reset();

        canvasForStealingBegin = UnityEngine.GameObject.Find("CanvasForStealingBegin") as UnityEngine.GameObject;
    }

    public override void StealingBegin()
    {
        canvasForStealingBegin.SetActive(false);
        // 魔术师出场
        UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
        UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;

        Cell magician_birth = Globals.map.entryOfMaze;
        magician.name = "Mage_Girl";
        magician.transform.position = magician_birth.GetFloorPos();
        base.StealingBegin();        
    }

    public override void MagicianLifeOver()
    {
        Globals.canvasForMagician.tutorialText.gameObject.SetActive(false);
        Globals.canvasForMagician.RestartText.gameObject.SetActive(true);
        // to do : Enchance the time number
        base.MagicianLifeOver();
        InvokeRepeating("RestartCount", 0.0f,1.0f);        
    }

    void RestartCount()
    {
        if (restartInSeconds >= 0)
        {
            Globals.canvasForMagician.RestartText.text = "<b><color=red><size=30>" + restartInSeconds.ToString() + "</size></color></b> 秒后重新开始\n\n 教程关卡你都能输，能专心点儿么 =.= ";
            --restartInSeconds;
        }
        else
        {
            Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Level_1");
            CancelInvoke("RestartCount");
        }        
    }

    public override void LevelPassed()
    {
        base.LevelPassed();
        Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");
    }

    public void LeaveBeforeStealing()
    {
        canvasForStealingBegin.SetActive(false);
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }
}
