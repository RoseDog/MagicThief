public class Tutorial_Level_1_Controller : TutorialLevelController 
{
    int restartInSeconds = 8;
    public override void MazeFinished()
    {
        base.MazeFinished();
        StealingBegin();        
        Globals.canvasForMagician.tutorialText.gameObject.SetActive(true);        
        Globals.canvasForMagician.tutorialText.text = "小心这些守卫";
        Globals.canvasForMagician.RestartText.gameObject.SetActive(false);
        Globals.canvasForMagician.lifeNumber.Reset();
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
            Globals.canvasForMagician.RestartText.text = "<b><color=red><size=30>" + restartInSeconds.ToString() + "</size></color></b> 秒后重新开始\n\n 这是教程的第二关。。我该怎么吐槽你 =..= ";
            --restartInSeconds;
        }
        else
        {
            Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Level_1");
            CancelInvoke("RestartCount");
        }        
    }
}
