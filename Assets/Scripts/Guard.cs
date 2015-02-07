using System.Collections;

public class Guard : Actor 
{
    public Pathfinding.Node birthNode;    
    public Patrol patrol;
    public Chase chase;
    public Spot spot;
    public GuardAttack atk;
    public WanderingLostTarget wandering;
    public BackToBirthCell backing;
    public UnityEngine.Canvas canvasForCommandBtns;
    UnityEngine.UI.Button CancelHireBtn;
    UnityEngine.UI.Button GuardInfoBtn;
    public FOV2DEyes[] eyes;
    public GuardAlertSound alertSound;
    public HeardAlert heardAlert;
    public GoCoveringTeammate goCovering;
    public Distraction distraction;
    public BeenHypnosised beenHypnosised;
    public WakeUp wakeFromHypnosised;
    public RealiseGemLost realiseGemLost;    

    [UnityEngine.HideInInspector]
    public bool walkable;

    [UnityEngine.HideInInspector]
    public float magicianOutVisionTime;
    [UnityEngine.HideInInspector]
    public float atkCd = 2.0f;
    [UnityEngine.HideInInspector]
    public int attackValue = 60;
    [UnityEngine.HideInInspector]
    public float atkShortestDistance = 3.0f;
    [UnityEngine.HideInInspector]
    public float doveOutVisionTime = 1.0f;
    [UnityEngine.HideInInspector]
    public float attackSpeed;

    UnityEngine.GameObject defenderArrangeUIPrefab;
    UnityEngine.GameObject challengerTricksUIPrefab;

    public UnityEngine.GameObject guardedGemHolder = null;

    public GuardHireInfo hireInfo;
    public override void Awake()
    {                
        patrol = GetComponent<Patrol>();
        chase = GetComponent<Chase>();
        spot = GetComponent<Spot>();
        atk = GetComponent<GuardAttack>();
        wandering = GetComponent<WanderingLostTarget>();
        realiseGemLost = GetComponent<RealiseGemLost>();
        backing = GetComponent<BackToBirthCell>();        
        eyes = GetComponentsInChildren<FOV2DEyes>();
        alertSound = GetComponent<GuardAlertSound>();
        heardAlert = GetComponent<HeardAlert>();
        goCovering = GetComponent<GoCoveringTeammate>();
        distraction = GetComponent<Distraction>();
        beenHypnosised = GetComponent<BeenHypnosised>();
        wakeFromHypnosised = GetComponent<WakeUp>();
        Globals.maze.guards.Add(this);

        magicianOutVisionTime = 2.3f;
        atkCd = 2.0f;
        attackValue = 60;
        atkShortestDistance = 3.0f;
        doveOutVisionTime = 1.0f;
        attackSpeed = 1.0f;
        walkable = true;
        base.Awake();
    }

    public override void Start()
    {
        if (anim != null)
        {
            anim["A"].speed = attackSpeed;
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
            foreach (FOV2DEyes eye in eyes)
            {
                if (eye.fovMaxDistance > fovMaxDistance)
                {
                    fovMaxDistance = eye.fovMaxDistance;
                }
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
                    new Sequence(new ScaleTo(transform, new UnityEngine.Vector3(1.6f, 1.6f, 1.6f), 0.1f),
                        new ScaleTo(transform, scaleCache, 0.1f))
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
            ConfirmHireBtn.onClick.AddListener(() => ConfirmBtnClicked());

            GuardInfoBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "GuardInfoBtn");
            CancelHireBtn = Globals.getChildGameObject<UnityEngine.UI.Button>(obj, "CancelHireBtn");

            if (everConfirmed)
            {
                CancelHireBtn.gameObject.SetActive(false);
                GuardInfoBtn.onClick.AddListener(() => GuardInfoBtnClicked());            
            }
            else
            {
                GuardInfoBtn.gameObject.SetActive(false);
                CancelHireBtn.onClick.AddListener(() => CancelHireBtnClicked());
            }                        

            isShownBtn = true;
            canvasForCommandBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 1.0f, 0.0f);
            canvasForCommandBtns.GetComponent<Actor>().AddAction(
                new ScaleTo(canvasForCommandBtns.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 0.2f));            
        }        
    }

    bool everConfirmed = false;
    public void ConfirmBtnClicked()
    {        
        if (birthNode.walkable == walkable)
        {
            everConfirmed = true;
            if (hireInfo != null)
            {
                hireInfo.hired = true;
            }
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
        Globals.DestroyGuard(this);        
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
            if(beenHypnosised != null)
            {
                btn.guard = this;
            }
            else
            {
                btn.gameObject.SetActive(false);
            }

            isShownBtn = true;            
        }
    }
    

    public void HideBtns()
    {
        isShownBtn = false;
        if (canvasForCommandBtns != null)
        {
            Destroy(canvasForCommandBtns.gameObject);
            canvasForCommandBtns = null;
        }        
    }

    public void EnableEyes(bool enable)
    {
        foreach (FOV2DEyes eye in eyes)
        {
            eye.gameObject.SetActive(enable);
        }
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
        anim.CrossFade("idle");
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
        else if(currentAction == chase)
        {
            atk.Excute();
        }
        else if (currentAction == goCovering)
        {
            wandering.Excute();
        }
    }

    public void FaceTarget(UnityEngine.Transform target)
    {
        UnityEngine.Vector3 horDir = GetDirToTarget(target);
        transform.forward = horDir;
    }

    public UnityEngine.Vector3 GetDirToTarget(UnityEngine.Transform target)
    {
        UnityEngine.Vector3 horDir = target.transform.position - transform.position;
        horDir.y = 0;
        return horDir;
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
	public override void FixedUpdate () 
    {
		base.FixedUpdate();
		if (canvasForCommandBtns != null)
        {
            canvasForCommandBtns.transform.position = transform.position + new UnityEngine.Vector3(0.0f, 1.0f, 0.0f);
        }        
    }   

    public virtual void SpotEnemy(UnityEngine.GameObject gameObj)
    {
        if(!eyes[0].gameObject.activeSelf)
        {
            return;
        }
        if (gameObj.layer == 11)
        {
            spot.SpotMagician(gameObj, magicianOutVisionTime, true);
        }
        else if (gameObj.layer == 20)
        {
            spot.SpotMagician(gameObj, doveOutVisionTime, true);
        }
    }

    public bool IsSeenEnemy(UnityEngine.GameObject enemy)
    {
        // 如果守卫面对着魔术师
        UnityEngine.Vector3 magicianDir = enemy.transform.position - transform.position;
        magicianDir.y = 0;
        UnityEngine.Vector3 faceDir = transform.forward;
        faceDir.y = 0;
        float angle = UnityEngine.Vector3.Angle(magicianDir, faceDir);
        if (angle < 90 && angle > -90)
        {
            // 而且中间没有任何墙体挡住
            return IsBlockByWall(enemy);
        }

        return false;
    }

    public bool IsBlockByWall(UnityEngine.GameObject enemy)
    {
        UnityEngine.Vector3 magicianDir = enemy.transform.position - transform.position;
        magicianDir.y = 0;
        UnityEngine.RaycastHit hitInfo;
        int layermask = 1 << 8;
        UnityEngine.Ray ray = new UnityEngine.Ray(transform.position, magicianDir);
        if (!UnityEngine.Physics.Raycast(ray, out hitInfo, magicianDir.magnitude, layermask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
