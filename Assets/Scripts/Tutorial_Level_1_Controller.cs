public class Tutorial_Level_1_Controller : TutorialLevelController 
{
    UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 8.0f, 0.0f);
    UnityEngine.Vector3 cameraOffsetCache;
    float fallingDuration = 1.5f;

    int restartInSeconds = 8;
    public UnityEngine.GameObject canvasForStealingBegin;
    UnityEngine.UI.Button LeaveBtn;
    UnityEngine.UI.Button StealingBtn;
    
    public override void Awake()
    {
        canvasForStealingBegin = UnityEngine.GameObject.Find("CanvasForStealingBegin") as UnityEngine.GameObject;        
        StealingBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "StealingBtn");
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "LeaveBtn");
        canvasForStealingBegin.SetActive(false);

        base.Awake();
    }
    public override void Start()
    {
        mapIniFileName = "Tutorial_Level_" + Globals.LevelIdx.ToString();
        UnityEngine.Debug.Log("map file:"+mapIniFileName);                
        base.Start();
    }
    public override void MazeFinished()
    {
        base.MazeFinished();

        if (Globals.LevelIdx == 0)
        {
            // 相机就位
            Globals.cameraFollowMagician.transform.position = Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f;
            Globals.cameraFollowMagician.enabled = false;
            StartCoroutine(CamLookAtMagicianDuringFalling());

            Globals.transition.BlackIn();

            // 主角降下            
            Globals.magician.Falling(Globals.magician.transform.position + posOnSky,
                Globals.magician.transform.position,
                fallingDuration);
            // 禁止输入        
            Globals.EnableAllInput(false);
        }
        else
        {                        
            Globals.EnableAllInput(true);
            Globals.canvasForMagician.SetLifeVisible(true);
            Globals.canvasForMagician.RestartText.gameObject.SetActive(false);
            Globals.canvasForMagician.lifeNumber.Reset();
            
            // 有守卫，要点了潜入才能开始
            if (Globals.map.guardsOnMap.Count != 0)
            {
                Globals.magician.gameObject.SetActive(false);
                canvasForStealingBegin.SetActive(true);
                Globals.joystick.MannullyActive(false);
            }
            // 没有守卫，不需要潜入按钮，直接开始
            else
            {
                canvasForStealingBegin.SetActive(false);
                StealingBegin();
            }

            if (Globals.asyncLoad.LastLevelName == "City")
            {
                LeaveBtn.gameObject.SetActive(true);
            }
            else
            {
                LeaveBtn.gameObject.SetActive(false);
            }
        }
        ++Globals.LevelIdx;
    }

    System.Collections.IEnumerator CamLookAtMagicianDuringFalling()
    {
        float time = fallingDuration;
        while (time > 0.0f)
        {
            time = time - UnityEngine.Time.deltaTime;
            Globals.cameraFollowMagician.transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));
            yield return null;
        }
        Invoke("LandingOver", 1.5f);
        yield return null;
    }

    void LandingOver()
    {
        Globals.transition.BlackOut(this, "StealingBegin");
    }

    public override void StealingBegin()
    {
        canvasForStealingBegin.SetActive(false);        
        base.StealingBegin();        
    }

    public void LeaveBeforeStealing()
    {
        canvasForStealingBegin.SetActive(false);
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public override void MagicianLifeOver()
    {
        Globals.canvasForMagician.RestartText.gameObject.SetActive(true);
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
            CancelInvoke("RestartCount");
            --Globals.LevelIdx;
            LevelPassed();
        }        
    }

    public override void LevelPassed()
    {
        base.LevelPassed();
        UnityEngine.Debug.Log("level passed");
        if (Globals.LevelIdx == 4)
        {
            Globals.transition.BlackOut(this, "Newsreport");            
        }
        else if (Globals.LevelIdx == 3)
        {
            canvasForStealingBegin.SetActive(false);
            Globals.asyncLoad.ToLoadSceneAsync("City");
        }
        else
        {
            LevelTipBillboard.SetActive(false);
            Globals.cameraFollowMagician.beginFollow(null);
            Globals.magician.gameObject.SetActive(false);
            Globals.map.ClearMaze();
            Start();
            Globals.map.Start();
        }
    }

    UIMover[] papers;
    void Newsreport()
    {
        UnityEngine.Debug.Log("Newsreport");
        // 禁止输入        
        Globals.EnableAllInput(false);       

        UnityEngine.GameObject MoonNightThief_prefab = UnityEngine.Resources.Load("MoonNightThief") as UnityEngine.GameObject;
        UnityEngine.GameObject MoonNightThief = UnityEngine.GameObject.Instantiate(MoonNightThief_prefab) as UnityEngine.GameObject;
        // 远离迷宫
        MoonNightThief.transform.position = new UnityEngine.Vector3(1000,0,1000);

        // 站立点
        UnityEngine.GameObject Stand = Globals.getChildGameObject(MoonNightThief, "StandPos");        
        Globals.magician.transform.parent = Stand.transform.parent;
        Globals.magician.transform.position = Stand.transform.position;
        Globals.magician.transform.localRotation = Stand.transform.localRotation;
        Globals.magician.transform.localScale = Stand.transform.localScale;
        Globals.magician.CoverInMoonlight();
        
        // 相机就位
        UnityEngine.GameObject cam = Globals.getChildGameObject(MoonNightThief, "Camera");
        Globals.cameraFollowMagician.transform.parent = cam.transform.parent;
        Globals.cameraFollowMagician.transform.position = cam.transform.position;
        Globals.cameraFollowMagician.transform.localRotation = cam.transform.localRotation;
        Globals.cameraFollowMagician.enabled = false;
        Globals.cameraFollowMagician.transform.LookAt(Stand.transform.position);               

        // 报纸报道
        // Newsreport.Show();
        papers = MoonNightThief.GetComponentsInChildren<UIMover>();
        Globals.transition.BlackIn(this, "NewsreportOut");        
    }

    void NewsreportOut()
    {
        StartCoroutine(_NewsreportOut());
    }

    System.Collections.IEnumerator _NewsreportOut()
    {
        foreach(UIMover paper in papers)
        {
            paper.BeginMove();
            yield return new UnityEngine.WaitForSeconds(paper.movingDuration);
        }

        //Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");
    }
}