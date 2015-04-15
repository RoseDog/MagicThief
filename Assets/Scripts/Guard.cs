using System.Collections;

public class Guard : Actor, System.IComparable<Guard>
{
    public Pathfinding.Node birthNode;    
    public Patrol patrol;
    public Chase chase;
    public Spot spot;
    public GuardAttack atk;
    public RushAtMagician rushAt;
    public WanderingLostTarget wandering;
    public BackToBirthCell backing;
    public UnityEngine.Canvas canvasForCommandBtns;
    UnityEngine.UI.Button CancelHireBtn;
    UnityEngine.UI.Button GuardInfoBtn;    
    public GuardAlertSound alertSound;
    public HeardAlert heardAlert;
    public GoCoveringTeammate goCovering;
    public Distraction distraction;
    public BeenHypnosised beenHypnosised;
    public WakeUp wakeFromHypnosised;
    public RealiseGemLost realiseGemLost;    

    [UnityEngine.HideInInspector]
    public bool walkable;
    

    UnityEngine.GameObject defenderArrangeUIPrefab;
    UnityEngine.GameObject challengerTricksUIPrefab;

    public UnityEngine.GameObject guardedGemHolder = null;

    public GuardData data;
    int idx;
    protected bool bGoChaseDove;
    public override void Awake()
    {                
        patrol = GetComponent<Patrol>();
        chase = GetComponent<Chase>();
        spot = GetComponent<Spot>();
        atk = GetComponent<GuardAttack>();
        rushAt = GetComponent<RushAtMagician>();
        wandering = GetComponent<WanderingLostTarget>();
        realiseGemLost = GetComponent<RealiseGemLost>();
        backing = GetComponent<BackToBirthCell>();                
        alertSound = GetComponent<GuardAlertSound>();
        heardAlert = GetComponent<HeardAlert>();
        goCovering = GetComponent<GoCoveringTeammate>();
        distraction = GetComponent<Distraction>();
        beenHypnosised = GetComponent<BeenHypnosised>();
        wakeFromHypnosised = GetComponent<WakeUp>();
        Globals.maze.guards.Add(this);

        idx = Globals.maze.guards.Count;

        
        walkable = true;
        bGoChaseDove = false;
        base.Awake();

        spriteSheet.CreateAnimationByName("idle");
        if(moving != null)
        {
            spriteSheet.CreateAnimationByName("moving");
        }        
    }

    public override void Start()
    {
        if(Globals.DEBUG_REPLAY)
        {
            gameObject.name += idx.ToString();
        }        
        base.Start();
    }

    public void OnDestroy()
    {
        if (currentAction != null)
        {
            currentAction.Stop();
        }
        Globals.maze.guards.Remove(this);        
    }

    public void InitArrangeUI()
    {
        defenderArrangeUIPrefab = UnityEngine.Resources.Load("Avatar/CanvasOnGuard") as UnityEngine.GameObject;        
    }

    public void InitTricksUI()
    {
        challengerTricksUIPrefab = UnityEngine.Resources.Load("Avatar/TricksOnGuard") as UnityEngine.GameObject;        
    }

    public void FindGuardedGem()
    {
        // 找到自己在守护的那颗宝石
        if (patrol != null)
        {
            // 找出最远的视野
            float fovMaxDistance = UnityEngine.Mathf.NegativeInfinity;
            if (eye.fovMaxDistance > fovMaxDistance)
            {
                fovMaxDistance = eye.fovMaxDistance;
            }

            // 先用简单的算法。如果四个巡逻点以及出生点中，其中一个点距离宝石小于了fovMaxDistance，那这颗宝石就是这个守卫在守护的宝石，
            System.Collections.Generic.List<UnityEngine.Vector3> poses = new System.Collections.Generic.List<UnityEngine.Vector3>(patrol.routePoses);
            poses.Add(Globals.GetPathNodePos(birthNode));
            foreach (UnityEngine.GameObject gem in Globals.maze.gemHolders)
            {
                foreach (UnityEngine.Vector3 pos in poses)
                {
                    float dis = UnityEngine.Vector3.Distance(pos, gem.transform.position);
                    if (dis < fovMaxDistance)
                    {
                        guardedGemHolder = gem;
                        break;
                        // to do: 找到最近的那一颗宝石
                        // fovMaxDistance = dis;
                    }
                }
            }
        }
    }

    public void Choosen()
    {
        UnityEngine.Debug.Log("Choosen");
        Globals.LevelController.GuardChoosen(this);
        ClearAllActions();
        AddAction(
                new Cocos2dParallel(
                    new Sequence(new ScaleTo(transform, new UnityEngine.Vector3(1.6f, 1.6f, 1.6f), 5),
                        new ScaleTo(transform, scaleCache, 5))
                        )
                        );
        ShowArrangeBtns();
        Tint();
        if (patrol != null)
        {
            patrol.SetRouteCubesVisible(true);
        }
        
        if (currentAction != null)
        {
            currentAction.Stop();
        }        
        Globals.maze.choosenGuard = this;
    }

    public void Unchoose()
    {
        UnityEngine.Debug.Log("unchoose");
        Globals.self.UploadGuards();
        StopTint();
        HideBtns();
        BeginPatrol();
        Globals.maze.choosenGuard = null;        
    }

    [UnityEngine.HideInInspector]
    public bool isShownBtn = false;
    public void ShowArrangeBtns()
    {
        if (!isShownBtn)
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(defenderArrangeUIPrefab) as UnityEngine.GameObject;
            canvasForCommandBtns = obj.GetComponent<UnityEngine.Canvas>();
            canvasForCommandBtns.worldCamera = Globals.cameraFollowMagician.camera;

            UnityEngine.UI.Button ConfirmHireBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "ConfirmHireBtn");
            

            GuardInfoBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "GuardInfoBtn");
            CancelHireBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "CancelHireBtn");
            
            GuardInfoBtn.onClick.AddListener(() => GuardInfoBtnClicked());
            ConfirmHireBtn.onClick.AddListener(() => ConfirmBtnClicked());
            CancelHireBtn.onClick.AddListener(() => CancelHireBtnClicked());            
            

            isShownBtn = true;
            canvasForCommandBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 1.0f, 0.0f);
            canvasForCommandBtns.GetComponent<Actor>().AddAction(
                new ScaleTo(canvasForCommandBtns.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 7));            
        }        
    }

    bool everConfirmed = false;
    public void ConfirmBtnClicked()
    {        
        if (birthNode.walkable == walkable)
        {
            everConfirmed = true;            
            Unchoose();
            Globals.LevelController.GuardDropped(this);
        }
        else
        {
            if (walkable)
            {
                Globals.tipDisplay.Msg("guard_cant_be_on_wall");
            }
            else
            {
                Globals.tipDisplay.Msg("lamp_cant_be_on_road");
            }
        }
    }

    public void CancelHireBtnClicked()
    {
        HideBtns();
        if ((Globals.LevelController as MyMazeLevelController) != null)
        {
            Globals.canvasForMyMaze.ChangeMazeRoom(-data.roomConsume);
        }
        
        Globals.DestroyGuard(this);
        Globals.self.UploadGuards();
    }

    public void GuardInfoBtnClicked()
    {
        UnityEngine.GameObject GuardInfoUI_prefab = UnityEngine.Resources.Load("UI/GuardInfoUI") as UnityEngine.GameObject;
        GuardInfoUI info = (UnityEngine.GameObject.Instantiate(GuardInfoUI_prefab) as UnityEngine.GameObject).GetComponentInChildren<GuardInfoUI>();
        info.SetGuard(this);
    }

    public void ShowTrickBtns()
    {
        if (!isShownBtn)
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(challengerTricksUIPrefab) as UnityEngine.GameObject;
            canvasForCommandBtns = obj.GetComponent<UnityEngine.Canvas>();
            canvasForCommandBtns.worldCamera = Globals.cameraFollowMagician.camera;

            TrickBtnOnGuardHead btn = obj.GetComponentInChildren<TrickBtnOnGuardHead>();
            btn.guard = this;            
            isShownBtn = true;            
        }
    }
    

    public void HideBtns()
    {
        isShownBtn = false;
        if (canvasForCommandBtns != null)
        {
            DestroyImmediate(canvasForCommandBtns.gameObject);
            canvasForCommandBtns = null;
        }        
    }

    public void EnableEyes(bool enable)
    {
        eye.gameObject.SetActive(enable);
    }

    public void BeginPatrol()
    {
        everConfirmed = true;
        if (patrol != null)
        {
            EnableEyes(true);
            patrol.RouteConfirmed();
            patrol.SetRouteCubesVisible(false);
            patrol.Excute();
        }        
    }

    public void StopAttacking()
    {
        EnableEyes(false);        
        currentAction.Stop();
        spriteSheet.Play("idle");
    }

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        if (currentAction == patrol)
        {
            patrol.NextPatrolTargetPos();
        }
        else if (currentAction == backing)
        {
            patrol.Excute();
        }        
        else if (currentAction == goCovering)
        {
            wandering.Excute();
        }
    }    

    bool tinting;
    public void Tint()
    {
        tinting = true;
        StartCoroutine(_tint());
    }

    public void StopTint()
    {
        tinting = false;        
    }

    float tintCurrentTime = 1.0f;
    float tintFadeTime = 0.3f;
    IEnumerator _tint()
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        tintCurrentTime = tintFadeTime;
        while (color.r > 0.3f)
        {
            tintCurrentTime = tintCurrentTime - UnityEngine.Time.deltaTime;
            color.r = tintCurrentTime / tintFadeTime;
            if (birthNode != null && birthNode.walkable)
            {                
                color.g = tintCurrentTime / tintFadeTime;
                color.b = tintCurrentTime / tintFadeTime;
            }
            else
            {
                color.g = 0.0f;
                color.b = 0.0f;                
            }
            
            SetColor(color);
            yield return null;
        }

        if (tinting)
        {
            yield return StartCoroutine(_tintBack());
        }
        else
        {
            SetColor(UnityEngine.Color.white);
            yield return null;
        }        
    }


    IEnumerator _tintBack()
    {
        UnityEngine.Color color = UnityEngine.Color.black;
        tintCurrentTime = color.r;
        while (color.r < 1.0f)
        {
            tintCurrentTime = tintCurrentTime + UnityEngine.Time.deltaTime;
            color.r = tintCurrentTime / tintFadeTime;
            color.g = tintCurrentTime / tintFadeTime;
            color.b = tintCurrentTime / tintFadeTime;
            SetColor(color);
            yield return null;
        }

        if (tinting)
        {
            yield return StartCoroutine(_tint());
        }
        else
        {
            SetColor(UnityEngine.Color.white);
            yield return null;
        }        
    }

    void SetColor(UnityEngine.Color color)
    {
        for (int idx = 0; idx < meshRenderers.Length; ++idx)
        {
            meshRenderers[idx].material.SetColor("_Color", color);
        }

        for (int idx = 0; idx < skinnedMeshRenderers.Length; ++idx)
        {
            skinnedMeshRenderers[idx].material.SetColor("_Color", color);
        }
    }
    
	// Update is called once per frame
	public override void Update () 
    {
		base.Update();
		if (canvasForCommandBtns != null)
        {
            canvasForCommandBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 1.0f, -0.6f);
        }        
    }   

    public void SpotEnemy(UnityEngine.GameObject gameObj)
    {
        // 如果是宝石，检查是否被偷
        if (realiseGemLost != null &&
            realiseGemLost != currentAction &&
            spot.target == null &&
            gameObj == guardedGemHolder)
        {
            if (gameObject.layer == 24 && gameObj.GetComponentInChildren<Gem>() == null)
            {
                realiseGemLost.Excute();
                return;
            }
        }
        
        if (gameObj.layer == 11)
        {
            spot.SpotMagician(gameObj, true);
        }
        else if (gameObj.layer == 20)
        {
            spot.SpotMagician(gameObj, bGoChaseDove);
        }
    }

    public virtual void EnemyOutEye(UnityEngine.GameObject gameObj)
    {
        if (gameObj.transform == spot.target)
        {
            if (gameObj.layer == 11)
            {
                spot.EnemyOutVision(data.magicianOutVisionTime);
            }
            else if (gameObj.layer == 20)
            {
                spot.EnemyOutVision(data.doveOutVisionTime);
            }
        }       
    }

    public void EnemyStayInEye(UnityEngine.GameObject gameObj)
    {
        if(currentAction == wandering)
        {
            SpotEnemy(gameObj);
        }
    }

    public bool IsSeenEnemy(UnityEngine.GameObject enemy)
    {
        // 如果守卫面对着魔术师
        UnityEngine.Vector3 magicianDir = enemy.transform.position - transform.position;
        magicianDir.z = 0;
        UnityEngine.Vector3 faceDir = moving.currentDir;
        faceDir.z = 0;
        float angle = UnityEngine.Vector3.Angle(magicianDir, faceDir);
        if (angle < 90 && angle > -90)
        {
            // 而且中间没有任何墙体挡住
            return !IsBlockByWall(enemy);
        }

        return false;
    }

    public bool IsBlockByWall(UnityEngine.GameObject enemy)
    {
        UnityEngine.Vector3 magicianDir = enemy.transform.position - transform.position;
        magicianDir.z = 0;
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 8;
        float collide_radius = controller.radius * transform.localScale.x * 3.5f;
        UnityEngine.Ray ray = new UnityEngine.Ray(transform.position, magicianDir + new UnityEngine.Vector3(collide_radius,0,0));
        UnityEngine.Ray ray1 = new UnityEngine.Ray(transform.position, magicianDir + new UnityEngine.Vector3(-collide_radius,0,0));
        UnityEngine.Ray ray2 = new UnityEngine.Ray(transform.position, magicianDir + new UnityEngine.Vector3(0, collide_radius, 0));
        UnityEngine.Ray ray3 = new UnityEngine.Ray(transform.position, magicianDir + new UnityEngine.Vector3(0, -collide_radius, 0));
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, magicianDir.magnitude, layermask) ||
            UnityEngine.Physics.Raycast(ray1, out hitInfo, magicianDir.magnitude, layermask) ||
            UnityEngine.Physics.Raycast(ray2, out hitInfo, magicianDir.magnitude, layermask) ||
            UnityEngine.Physics.Raycast(ray3, out hitInfo, magicianDir.magnitude, layermask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsBlockByWallLossCheck(UnityEngine.GameObject enemy)
    {
        UnityEngine.Vector3 magicianDir = enemy.transform.position - transform.position;
        magicianDir.z = 0;
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 8;
        UnityEngine.Ray ray = new UnityEngine.Ray(transform.position, magicianDir);        
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, magicianDir.magnitude, layermask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int CompareTo(Guard other)
    {
        if (other == this)
        {
            return 0;
        }
        if (UnityEngine.Vector3.Distance(transform.position, Globals.magician.transform.position) >
            UnityEngine.Vector3.Distance(other.transform.position, Globals.magician.transform.position))
        {
            return 1;
        }
        return -1;
    }
}
