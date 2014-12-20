public class TutorialLevelController : LevelController
{    
    protected UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 8.0f, 0.0f);
    protected float fallingDuration = 1.5f;
        
    int restartInSeconds = 3;
    int countDownSeconds;

    public UnityEngine.GameObject canvasForStealingBegin;
    public LevelTip LevelTip;
    UnityEngine.UI.Button LeaveBtn;
    UnityEngine.UI.Button StealingBtn;
    public Cash StealingCash;
    public System.Collections.Generic.List<UnityEngine.GameObject> coinsOnFloor = new System.Collections.Generic.List<UnityEngine.GameObject>();

    public float paperMovingDuration = 1.2f;

    public override void Awake()
    {
        base.Awake();
        countDownSeconds = restartInSeconds;

        canvasForStealingBegin = UnityEngine.GameObject.Find("CanvasForStealingBegin") as UnityEngine.GameObject;
        LevelTip = Globals.getChildGameObject<LevelTip>(canvasForStealingBegin, "LevelTip");
        LevelTip.gameObject.SetActive(false);
        StealingBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "StealingBtn");
        LeaveBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(canvasForStealingBegin, "LeaveBtn");
        StealingCash = Globals.getChildGameObject<Cash>(canvasForStealingBegin, "StealingCash");

        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.Over)
        {
            mazeIniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        }
        else
        {
            mazeIniFileName = "Tutorial_Level_" + Globals.TutorialLevel.Guard;
        }
        
        UnityEngine.Debug.Log("map file:" + mazeIniFileName);
    }

    public override void MazeFinished()
    {
        base.MazeFinished();                
        if (Globals.magician == null)
        {
            // 魔术师出场
            UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
            UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;
        }
       
        Globals.magician.transform.position = Globals.maze.entryOfMaze.GetFloorPos();
        UnityEngine.Debug.Log(Globals.magician.transform.position.y.ToString("F4"));
        Globals.maze.RegistChallengerEvent();


        Globals.cameraFollowMagician.Reset();
        Globals.transition.fadeColor.a = 1.0f;
        Globals.transition.BlackIn();

        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstFalling)
        {
            // 隐藏界面            
            StealingCash.gameObject.SetActive(false);

            // 禁止输入        
            Globals.EnableAllInput(false);
            // 相机就位
            Globals.cameraFollowMagician.transform.position = Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f;

            // 角色落下
            MagicianFallingDown();
            Globals.cameraFollowMagician.StaringMagician(fallingDuration);
        }
        else
        {
            Globals.EnableAllInput(true);            
            Globals.canvasForMagician.RestartText.gameObject.SetActive(false);
            Globals.canvasForMagician.lifeNumber.Reset();

            // 有守卫，要点了潜入才能开始
            if (Globals.maze.guards.Count != 0)
            {
                Globals.canvasForMagician.SetLifeVisible(true);
                StealingBtn.gameObject.SetActive(true);
                Globals.magician.gameObject.SetActive(false);
                Globals.joystick.MannullyActive(false);
            }
            // 没有守卫，不需要潜入按钮，直接开始
            else
            {
                Globals.canvasForMagician.SetLifeVisible(false);
                StealingBtn.gameObject.SetActive(false);
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
        StealingBtn.gameObject.SetActive(false);
        LeaveBtn.gameObject.SetActive(false);
        Globals.magician.gameObject.SetActive(true);
        if (Globals.TutorialLevelIdx != Globals.TutorialLevel.FirstFalling)
        {
            // 相机跟随                    
            Globals.cameraFollowMagician.beginFollow(Globals.magician.transform);            
        }

        Globals.joystick.MannullyActive(false);

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
        else if (Globals.TutorialLevelIdx != Globals.TutorialLevel.FirstFalling)
        {
            ShowLevelTip();
        }
        else
        {
            // 妈比这个名字到底咋个取
            Invoke("TutorialOneMageGirlFallingOver", 1.5f);
        }
        
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
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.FirstFalling)
        {
            StealingCash.gameObject.SetActive(true);
            Globals.transition.BlackIn();
        }
        // 魔术师出场   
        Globals.magician.InStealing();                
    }

    public override void MagicianGotCash(float value)
    {
        Invoke("LevelPassed", 0.5f);
        StealingCash.Add(value);
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
        InvokeRepeating("RestartCount", 4.0f, 1.0f);
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
        ++Globals.TutorialLevelIdx;
        mazeIniFileName = "Tutorial_Level_" + Globals.TutorialLevelIdx.ToString();
        UnityEngine.Debug.Log("map file:" + mazeIniFileName);
        Globals.cashAmount += StealingCash.cashAmont;
        Globals.canvasForMagician.cashNumber.SetNumber(Globals.cashAmount);
        StealingCash.SetToZero();
        UnityEngine.Debug.Log("level passed:" + Globals.cashAmount.ToString());
        Globals.magician.victory.Excute();
    }

    public override void AfterMagicianSuccessedEscaped()
    {
        if (Globals.TutorialLevelIdx == Globals.TutorialLevel.InitMaze)
        {
            Globals.transition.BlackOut(this, "Newsreport");
        }
        else if (Globals.TutorialLevelIdx == Globals.TutorialLevel.MagicianBorn)
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

        Globals.asyncLoad.ToLoadSceneAsync("MagicianHome");
    }

    public void LeaveBeforeStealing()
    {
        canvasForStealingBegin.SetActive(false);
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }
}
