public class TutorialLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 8.0f, 0.0f);
    protected float fallingDuration = 1.5f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealingBegin;
    public LevelTip LevelTip;
    UnityEngine.UI.Button LeaveBtn;
    public Number StealingCash;
    UIMover TricksPanel;
    public System.Collections.Generic.List<UnityEngine.GameObject> coinsOnFloor = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public float paperMovingDuration = 1.2f;
    
    UnityEngine.Vector3 camOffsetInSpy = new UnityEngine.Vector3(0, 20, -7);
    UnityEngine.Vector3 camOffsetInStealing = new UnityEngine.Vector3(0, 21, -7);

    UnityEngine.GameObject mark_prefab;
    public UnityEngine.GameObject landingMark;

    public System.Collections.Generic.List<UnityEngine.GameObject> unstolenGems = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public override void Awake()
    {
        base.Awake();
        countDownSeconds = restartInSeconds;

        canvasForStealingBegin = UnityEngine.GameObject.Find("CanvasForStealingBegin") as UnityEngine.GameObject;
        LevelTip = Globals.getChildGameObject<LevelTip>(canvasForStealingBegin, "LevelTip");
        LevelTip.gameObject.SetActive(false);
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "LeaveBtn");
        LeaveBtn.gameObject.SetActive(false);
        StealingCash = Globals.getChildGameObject<Number>(canvasForStealingBegin, "StealingCash");
        StealingCash.gameObject.SetActive(false);
        TricksPanel = Globals.getChildGameObject<UIMover>(canvasForStealingBegin, "TricksPanel");

        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            Globals.iniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        }
        else
        {
            Globals.Assert(Globals.iniFileName != "");
            if (Globals.iniFileName == "")
            {
                Globals.iniFileName = "扑克脸";
            }            
        }        
                
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);


        mark_prefab = UnityEngine.Resources.Load("UI/LandingPositionMark") as UnityEngine.GameObject;
        landingMark = UnityEngine.GameObject.Instantiate(mark_prefab) as UnityEngine.GameObject;
        UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
        markBtn.onClick.AddListener(() => MagicianFallingDown());        
        markBtn.GetComponent<Actor>().AddAction(
            new RotateTo(
                new UnityEngine.Vector3(90.0f, 0.0f, 0.0f), 
                new UnityEngine.Vector3(90.0f, 360.0f, 0.0f), 3.0f, true));
        landingMark.SetActive(false);
    }

    public override void MazeFinished()
    {
        base.MazeFinished();                
        if (Globals.magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
            UnityEngine.GameObject.Instantiate(magician_prefab);
        }

        foreach(UnityEngine.GameObject gem in Globals.maze.gemHolders)
        {
            unstolenGems.Add(gem);
        }

        foreach(Guard guard in Globals.maze.guards)
        {
            guard.InitTricksUI();
        }
       
        Globals.magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        Globals.maze.RegistChallengerEvent();

        Globals.cameraFollowMagician.Reset();
        
        Globals.canvasForMagician.RoseNumberBg.SetActive(false);

        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetGem)
        {
            Globals.cameraFollowMagician.disOffset = camOffsetInStealing;
            // 隐藏界面            
            StealingCash.gameObject.SetActive(false);

            // 禁止输入        
            Globals.EnableAllInput(false);
            // 相机就位
            Globals.cameraFollowMagician.transform.position = 
                Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f + new UnityEngine.Vector3(0, 0.2f, 0);

            // 角色落下
            MagicianFallingDown();
            Globals.cameraFollowMagician.StaringMagician(fallingDuration);
        }
        else
        {
            Globals.EnableAllInput(true);
            Globals.canvasForMagician.SetLifeVisible(true);
            Globals.canvasForMagician.RestartText.gameObject.SetActive(false);
            Globals.magician.ResetLifeAndPower();

            Globals.cameraFollowMagician.disOffset = camOffsetInSpy;            

            // 有守卫，要点了潜入才能开始
            if (Globals.maze.guards.Count != 0)
            {
                
                Globals.magician.gameObject.SetActive(false);
            }
            // 没有守卫，不需要潜入按钮，直接开始
            else
            {
                // 主角降下          
                MagicianFallingDown();
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
    }

    public virtual void MagicianFallingDown()
    {
        UnityEngine.UI.Button markBtn = landingMark.GetComponentInChildren<UnityEngine.UI.Button>();
        markBtn.interactable = false;
        LeaveBtn.gameObject.SetActive(false);
        Globals.magician.gameObject.SetActive(true);        
        if(landingMark.activeSelf)
        {
            Globals.magician.transform.position = landingMark.transform.position;
        }        
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.GetGem)
        {
            // 相机跟随                    
            Globals.cameraFollowMagician.MoveToPoint(Globals.magician.transform.position, camOffsetInStealing, 1.0f);
        }

        // 主角降下     
        Globals.magician.falling.from = Globals.magician.transform.position + posOnSky;
        Globals.magician.falling.to = Globals.magician.transform.position;
        Globals.magician.falling.duration = fallingDuration;
        Globals.magician.falling.Excute();       
    }

    public override void AfterMagicianFalling()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            OperateMagician();
        }
        else if (Globals.TutorialLevelIdx != Globals.TutorialLevel.GetGem)
        {
            ShowLevelTip();
        }
        else
        {
            // 妈比这个名字到底咋个取
            Invoke("TutorialOneMageGirlFallingOver", 1.5f);
        }
        landingMark.SetActive(false);
        base.AfterMagicianFalling();
    }

    void TutorialOneMageGirlFallingOver()
    {
        Globals.transition.BlackOut(this, "ShowLevelTip");
    }

    public void ShowLevelTip()
    {
        // 关卡提示        
        Invoke("OperateMagician", LevelTip.GetFadeDuration() + LevelTip.GetWaitingDuration());
        LevelTip.Show(Globals.maze.LevelTipText);        
    }

    public void OperateMagician()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.GetGem)
        {            
            Globals.transition.BlackIn();
        }
        TricksPanel.BeginMove(Globals.uiMoveAndScaleDuration);

        UnityEngine.UI.Button[] btns = TricksPanel.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (UnityEngine.UI.Button btn in btns)
        {
            UnityEngine.UI.Button temp = btn.GetComponent<UnityEngine.UI.Button>();
            temp.onClick.AddListener(() => Globals.magician.TrickBtnClicked(temp));           
        }

        StealingCash.gameObject.SetActive(true);
        // 魔术师出场   
        Globals.magician.InStealing();                
    }

    public override void MagicianGotCash(float value)
    {
        //Invoke("LevelPassed", 0.5f);        
        base.MagicianGotCash(value);
    }

    public override void MagicianLifeOver()
    {
        float forceFactor = 2.0f;
        float rotateForceFactor = 115.0f;

        UnityEngine.GameObject dropPrefab = UnityEngine.Resources.Load("Props/DroppedCoin") as UnityEngine.GameObject;
        for (int i = 0; i < 15; ++i)
        {
            UnityEngine.GameObject coin = UnityEngine.GameObject.Instantiate(dropPrefab,
                Globals.magician.transform.position + new UnityEngine.Vector3(0, Globals.magician.controller.bounds.size.y * 0.5f, 0),
                Globals.magician.transform.rotation) as UnityEngine.GameObject;

            UnityEngine.Vector3 randForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(1.0f, 4.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f));
            randForce *= forceFactor;

            UnityEngine.Vector3 randRotForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1.0f, 1.0f),
                                                UnityEngine.Random.Range(-1.0f, 1.0f),
                                                UnityEngine.Random.Range(-1.0f, 1.0f));
            randRotForce *= rotateForceFactor;
            coin.rigidbody.velocity = randForce;
            coin.rigidbody.angularVelocity = randRotForce;
            coinsOnFloor.Add(coin);
        }


        StealingCash.SetToZero();
        base.MagicianLifeOver();
        if(Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            InvokeRepeating("RestartCount", 4.0f, 1.0f);
        }        
    }

    void RestartCount()
    {
        Globals.canvasForMagician.RestartText.gameObject.SetActive(true);
        if (countDownSeconds >= 0)
        {
            Globals.canvasForMagician.RestartText.text = "<b><color=red><size=30>" + countDownSeconds.ToString() + "</size></color></b> 秒后重新开始\n\n 教程关卡你都能输，能专心点儿么 =.= ";
            --countDownSeconds;
        }
        else
        {
            foreach (UnityEngine.GameObject coin in coinsOnFloor)
            {
                Destroy(coin);
            }
            coinsOnFloor.Clear();
            countDownSeconds = restartInSeconds;
            CancelInvoke("RestartCount");

            Globals.magician.gameObject.SetActive(false);
            Globals.maze.ClearMaze();
            Start();
            Globals.maze.Start();
        }
    }

    public override void LevelPassed()
    {
        base.LevelPassed();
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            ++Globals.TutorialLevelIdx;
        }        
        Globals.iniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        UnityEngine.Debug.Log("map file:" + Globals.iniFileName);
        Globals.cashAmount += StealingCash.numberAmont;
        Globals.canvasForMagician.cashNumber.SetNumber(Globals.cashAmount);
        StealingCash.SetToZero();
        UnityEngine.Debug.Log("level passed:" + Globals.cashAmount.ToString());
        Globals.magician.victory.Excute();
    }

    public override void AfterMagicianSuccessedEscaped()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.InitMyMaze || Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            canvasForStealingBegin.SetActive(false);
            Globals.transition.BlackOut(this, "Newsreport");
        }
        else if (Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstTarget)
        {
            canvasForStealingBegin.SetActive(false);
            Globals.asyncLoad.ToLoadSceneAsync("City");
        }
        else
        {
            Globals.magician.gameObject.SetActive(false);
            Globals.maze.ClearMaze();
            Start();
            Globals.maze.Start();
        }
        base.AfterMagicianSuccessedEscaped();
    }

    public override void AfterMagicianLifeOverEscaped()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            Leave();
        }
        base.AfterMagicianLifeOverEscaped();
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
        MoonNightThief.transform.position = new UnityEngine.Vector3(1000, 0, 1000);

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
        foreach (UIMover paper in papers)
        {
            paper.BeginMove(paperMovingDuration);
            yield return new UnityEngine.WaitForSeconds(paperMovingDuration);
        }
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.InitMyMaze)
        {
            Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");
        }
        else if (Globals.TutorialLevelIdx == Globals.TutorialLevel.Over)
        {
            Globals.asyncLoad.ToLoadSceneAsync("City");
            //增加需要救助的穷人;
            Globals.AddPoorBuildingAchives(Globals.currentStealingTargetBuildingAchive);            
        }        
    }

    public void Leave()
    {
        UnityEngine.Debug.Log("back to city");
        canvasForStealingBegin.SetActive(false);
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        foreach (Guard guard in Globals.maze.guards)
        {
            if (Globals.magician.Stealing  
                && UnityEngine.Vector3.Distance(Globals.magician.transform.position, guard.transform.position) < 7.0f
                && guard.currentAction != guard.beenHypnosised)
            {
                guard.ShowTrickBtns();                                         
            }
            else
            {
                guard.HideBtns();
            }   
        }               
    }
}
