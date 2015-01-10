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
    public BeginPatrolBtn beginPatrolBtn;
    public TakeGuardBack takeGuardBackBtn;    
    public FOV2DEyes[] eyes;
    public GuardAlertSound alertSound;
    public HeardAlert heardAlert;
    public GoCoveringTeammate goCovering;
    public Distraction distraction;
    UnityEngine.Vector3 scaleCache;

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

    UnityEngine.GameObject uiPrefab;
    public override void Awake()
    {        
        scaleCache = transform.localScale;
        patrol = GetComponent<Patrol>();
        chase = GetComponent<Chase>();
        spot = GetComponent<Spot>();
        atk = GetComponent<GuardAttack>();
        wandering = GetComponent<WanderingLostTarget>();
        backing = GetComponent<BackToBirthCell>();        
        eyes = GetComponentsInChildren<FOV2DEyes>();
        alertSound = GetComponent<GuardAlertSound>();
        heardAlert = GetComponent<HeardAlert>();
        goCovering = GetComponent<GoCoveringTeammate>();
        distraction = GetComponent<Distraction>();
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
        Globals.maze.guards.Remove(this);
    }

    public void InitArrangeUI()
    {
        uiPrefab = UnityEngine.Resources.Load("Avatar/CanvasOnGuard") as UnityEngine.GameObject;        
    }

    public void Choosen()
    {
        UnityEngine.Debug.Log("Choosen");
        ClearAllActions();
        AddAction(
                new Cocos2dParallel(
                    new Sequence(new ScaleTo(transform, new UnityEngine.Vector3(1.6f, 1.6f, 1.6f), 0.1f),
                        new ScaleTo(transform, scaleCache, 0.1f))
                        )
                        );
        ShowBtns();        
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

    public bool isShownBtns = true;
    public void ShowBtns()
    {
        if (!isShownBtns)
        {
            UnityEngine.GameObject obj = UnityEngine.GameObject.Instantiate(uiPrefab) as UnityEngine.GameObject;
            canvasForCommandBtns = obj.GetComponent<UnityEngine.Canvas>();
            canvasForCommandBtns.worldCamera = Globals.cameraFollowMagician.camera;
            beginPatrolBtn = obj.GetComponentInChildren<BeginPatrolBtn>();
            beginPatrolBtn.guard = this;
            beginPatrolBtn.patrol = patrol;

            takeGuardBackBtn = obj.GetComponentInChildren<TakeGuardBack>();
            takeGuardBackBtn.guard = this;

            isShownBtns = true;            
            canvasForCommandBtns.GetComponent<Actor>().AddAction(
                new ScaleTo(canvasForCommandBtns.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 0.2f));            
        }        
    }

    public void HideBtns()
    {
        isShownBtns = false;
        if (canvasForCommandBtns != null)
        {
            Destroy(canvasForCommandBtns.gameObject);
            canvasForCommandBtns = null;
        }        
    }

    public void BeginPatrol()
    {
        if (patrol != null)
        {
            foreach (FOV2DEyes eye in eyes)
            {
                eye.gameObject.SetActive(true);
            }
            patrol.RouteConfirmed();
            patrol.SetRouteCubesVisible(false);
            patrol.Excute();
        }        
    }

    public void StopAttacking()
    {
        foreach (FOV2DEyes eye in eyes)
        {
            eye.gameObject.SetActive(false);
        }
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
        if (gameObj.layer == 11)
        {
            spot.SpotMagician(gameObj, magicianOutVisionTime, true);
        }
        else if (gameObj.layer == 20)
        {
            spot.SpotMagician(gameObj, doveOutVisionTime, true);
        }
    }
}
