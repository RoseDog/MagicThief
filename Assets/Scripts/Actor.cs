public class Actor : UnityEngine.MonoBehaviour 
{
    public int update_eye_idx;
    [UnityEngine.HideInInspector]
    public UnityEngine.Animation anim;
    [UnityEngine.HideInInspector]
    public UnityEngine.SpriteRenderer spriteRenderer;
    [UnityEngine.HideInInspector]
    public SpriteSheet spriteSheet;
    [UnityEngine.HideInInspector]
    public UnityEngine.CharacterController characterController;
    [UnityEngine.HideInInspector]
    public UnityEngine.SpriteRenderer shadow;
    public System.Collections.Generic.List<Actor> actorsInTouch = new System.Collections.Generic.List<Actor>();

    public Action currentAction;

    [UnityEngine.HideInInspector]
    public Hitted hitted;
    [UnityEngine.HideInInspector]
    public LifeOver lifeOver;
    [UnityEngine.HideInInspector]
    public GuardMoving moving;
    [UnityEngine.HideInInspector]
    public BeenPressDown beenPressDown;
    [UnityEngine.HideInInspector]
    public CatchByNet catchByNet;

    [UnityEngine.HideInInspector]
    public UnityEngine.MeshRenderer[] meshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.SkinnedMeshRenderer[] skinnedMeshRenderers;
    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<UnityEngine.Renderer> renderers;

    UnityEngine.GameObject pathMeshPrefab;
    UnityEngine.GameObject pathMesh;

    [UnityEngine.HideInInspector]
    public bool bVisible = false;
    [UnityEngine.HideInInspector]
    public bool Stealing = false;

    [UnityEngine.HideInInspector]
    public bool inLight = false;

    [UnityEngine.HideInInspector]
    public int LifeAmount;

    [UnityEngine.HideInInspector]
    public float LifeCurrent;
    [UnityEngine.HideInInspector]
    public float PowerCurrent;
    [UnityEngine.HideInInspector]
    public UnityEngine.Vector3 scaleCache;
    [UnityEngine.HideInInspector]
    public double heightOriginCache;

    public FOV2DEyes eye;

    public UnityEngine.AudioSource audioSource;

    public UnityEngine.GameObject head_on_minimap;

    public static System.Collections.Generic.List<Actor> to_be_remove = new System.Collections.Generic.List<Actor>();    

    public bool isOpenChest = false;
    public bool isTakingMoneny = false;

    public virtual void Awake()
    {
        scaleCache = transform.localScale;
        heightOriginCache = transform.position.z;
        spriteRenderer = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(gameObject, "Sprite");
        shadow = Globals.getChildGameObject<UnityEngine.SpriteRenderer>(gameObject, "shadow");
        spriteSheet = GetComponentInChildren<SpriteSheet>();
        anim = GetComponent<UnityEngine.Animation>();
        characterController = GetComponent<UnityEngine.CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = true;
        }
        hitted = GetComponent<Hitted>();
        lifeOver = GetComponent<LifeOver>();
        moving = GetComponent<GuardMoving>();
        beenPressDown = GetComponent<BeenPressDown>();
        catchByNet = GetComponent<CatchByNet>();

        eye = GetComponentInChildren<FOV2DEyes>();

        meshRenderers = GetComponentsInChildren<UnityEngine.MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<UnityEngine.SkinnedMeshRenderer>();
        renderers = new System.Collections.Generic.List<UnityEngine.Renderer>(GetComponentsInChildren<UnityEngine.Renderer>());

        pathMeshPrefab = UnityEngine.Resources.Load("Misc/PathMesh") as UnityEngine.GameObject;

        audioSource = GetComponent<UnityEngine.AudioSource>();

        head_on_minimap = Globals.getChildGameObject(gameObject, "head_on_minimap");

        if (!Globals.to_add_actors.Contains(this))
        {
            Globals.to_add_actors.Add(this);
        }        
    }

    public virtual void OnDestroy()
    {
        Globals.actors.Remove(this);
    }

    public virtual void Start()
    {
        
    }

    public float GetWorldScale()
    {
        float world_scale = transform.localScale.x;
        if (transform.parent != null)
        {
            world_scale = world_scale * transform.parent.localScale.x;
        }
        return world_scale;
    }

    public float GetWorldRadius()
    {
        return characterController.radius * GetWorldScale();
    }

    public UnityEngine.Vector3 GetWorldCenterPos()
    {
        return transform.position + characterController.center * GetWorldScale();
    }

    public virtual void TouchBegin(Actor other)
    {
        actorsInTouch.Add(other);
    }

    public virtual void TouchStay(Actor other)
    {

    }

    public virtual void TouchOut(Actor other)
    {
        actorsInTouch.Remove(other);
    }

    public bool IsTouching(Actor other)
    {
        return actorsInTouch.Contains(other);       
    }

    public virtual void FrameFunc()
    {
        // Run actions
        UpdateActions();

        if ((Globals.LevelController.frameCount + update_eye_idx) % 5 == 0)
        {
            if (eye != null && eye.gameObject.activeSelf)
            {
                eye.FrameFunc();
            }
        }       

        if(currentAction != null)
        {
            currentAction.FrameFunc();
        }

        if(moving != null)
        {
            moving.FrameFunc();
        }

        if (spriteSheet != null)
        {
            spriteSheet.FrameFunc();
        }
    }
    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<Cocos2dAction> actions = new System.Collections.Generic.List<Cocos2dAction>();
    [UnityEngine.HideInInspector]
    System.Collections.Generic.List<Cocos2dAction> to_remove_actions = new System.Collections.Generic.List<Cocos2dAction>();
    [UnityEngine.HideInInspector]
    public System.Collections.Generic.List<Cocos2dAction> to_add_actions = new System.Collections.Generic.List<Cocos2dAction>();

    protected void UpdateActions()
    {
        foreach (Cocos2dAction action in to_remove_actions)
            to_add_actions.Remove(action);

        foreach (Cocos2dAction action in to_remove_actions)
            actions.Remove(action);
        to_remove_actions.Clear();

        foreach (Cocos2dAction action in to_add_actions)
            actions.Add(action);
        to_add_actions.Clear();        

        // Run actions
        if (actions.Count > 0)
        {
            to_remove_actions = new System.Collections.Generic.List<Cocos2dAction>();
            foreach (Cocos2dAction action in actions)
            {

                // Initialize action
                if (!action.IsInitialized())
                {
                    // Initialize action
                    action.Init();
                }

                if(!action.paused)
                {
                    action.Update();
                }

                // Remove action when completed
                if (action.IsCompleted()) 
                    to_remove_actions.Add(action);

            }
        }        
    }

    // Get amount of actions
    public int GetActionAmount()
    {
        // return actions count
        return actions.Count;
    }

    // Add Action
    public void AddAction(Cocos2dAction action)
    {        
        to_add_actions.Add(action);
        // Assign parent to action
        action.parent = this;
    }

    public void RemoveAction(ref Cocos2dAction action)
    {
        //to_add.Remove(action);
        to_remove_actions.Add(action);        
        action = null;
    }

    // Clear all Actions
    public void ClearAllActions()
    {
        actions.Clear();
    }

    public Sequence SleepThenCallFunction(int sleep, UnityEngine.Events.UnityAction a)
    {
        Sequence action = new Sequence(new SleepFor(sleep), new FunctionCall(a));
        AddAction(action);
        return action;
    }

    public RepeatForever RepeatingCallFunction(int sleep, UnityEngine.Events.UnityAction a)
    {
        RepeatForever action = new RepeatForever(new SleepFor(sleep), new FunctionCall(a));
        AddAction(action);
        return action;
    }

    public virtual void Visible(bool visibility)
    {
        foreach (UnityEngine.Renderer renderer in renderers)
        {
            renderer.enabled = visibility;
        }

        bVisible = visibility;
    }

    public bool IsVisible()
    {
        return renderers[0].enabled;
    }

    public bool IsLifeOver()
    {
        return lifeOver != null && (currentAction == lifeOver || LifeCurrent < UnityEngine.Mathf.Epsilon);
    }

    public virtual void InStealing()
    {
        Stealing = true;
        gameObject.layer = 11;
    }

    public virtual void OutStealing()
    {
        Stealing = false;
        gameObject.layer = 0;
    }

    public virtual bool GoTo(UnityEngine.Vector3 pos, OnPathDelegate callback = null)
    {
        if(callback == null)
        {
            callback = OnPathComplete;
        }
        System.String content = gameObject + " go to:";
        content += pos.ToString("F3");
        Globals.record("testReplay", content);

        moving.canSearch = false;
        moving.GetSeeker().StartPath(moving.GetFeetPosition(), pos, callback);
        return true;
    }

    public void OnPathComplete(Pathfinding.Path p)
    {
        System.String content = gameObject.name + "vPath:";
        System.Collections.Generic.List<UnityEngine.Vector3> vectorPath = new System.Collections.Generic.List<UnityEngine.Vector3>();
        foreach (UnityEngine.Vector3 pos in p.vectorPath)
        {
//             vectorPath.Add(new UnityEngine.Vector3(
//                 (double)System.Math.Round(pos.x, 3), (double)System.Math.Round(pos.y, 3), (double)System.Math.Round(pos.z, 3)));
            vectorPath.Add(pos);
        }
        p.vectorPath = vectorPath;

        foreach (UnityEngine.Vector3 pos in p.vectorPath)
        {
            content += pos.ToString("F5");
        }

        Globals.record("testReplay", content);
    }    

    public virtual void OnTargetReached()
    {
        
    }  

    void ShowPathComplete(Pathfinding.Path p)
    {
        System.Collections.Generic.List<UnityEngine.Vector3> path = p.vectorPath;        

         //先找出所有的拐点
        System.Collections.Generic.List<UnityEngine.Vector3> corners = new System.Collections.Generic.List<UnityEngine.Vector3>();
        corners.Add(path[0]);
        for (int i = 1; i < path.Count - 1; ++i)
        {
            // 如果x,z都跟下一个点不同 && x,z其中之一跟上一个点相同。那么是拐点
            if ((path[i].x == path[i - 1].x || path[i].y == path[i - 1].y) &&
                (path[i].x != path[i + 1].x && path[i].y != path[i + 1].y))
            {
                corners.Add(path[i]);
            }
        }
        corners.Add(path[path.Count - 1]);

        //System.Collections.Generic.List<UnityEngine.Vector3> corners = GetComponent<SimpleSmoothModifier>().SmoothOffsetSimple(path);

        float nodeSize = Globals.maze.pathFinder.graph.nodeSize;
        System.Collections.Generic.List<UnityEngine.Vector3> meshPoints = new System.Collections.Generic.List<UnityEngine.Vector3>();
        for (int i = 0; i < corners.Count; ++i)
        {
            UnityEngine.Vector3 point = corners[i];
            bool flag = false;
            if (i + 1 < corners.Count)
            {
                UnityEngine.Vector3 nextPoint = corners[i+1];
                if ((nextPoint.x < point.x && nextPoint.y < point.y)||
                    (nextPoint.x > point.x && nextPoint.y > point.y))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                meshPoints.Add(new UnityEngine.Vector3(point.x + nodeSize * 0.25f, point.y - nodeSize * 0.25f, 0));
                meshPoints.Add(new UnityEngine.Vector3(point.x - nodeSize * 0.25f, point.y + nodeSize * 0.25f, 0));
            }
            else
            {
                meshPoints.Add(new UnityEngine.Vector3(point.x + nodeSize * 0.25f, point.y + nodeSize * 0.25f, 0));
                meshPoints.Add(new UnityEngine.Vector3(point.x - nodeSize * 0.25f, point.y - nodeSize * 0.25f, 0));
            }            
        }

        if (pathMesh == null)
        {
            pathMesh = UnityEngine.GameObject.Instantiate(pathMeshPrefab) as UnityEngine.GameObject;
            pathMesh.transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y, 0);
        }
        pathMesh.GetComponent<PathMesh>().Construct(meshPoints);
    }

    public void ShowPathToPoint(UnityEngine.Vector3 destination)
    {
        GoTo(destination, ShowPathComplete);
        moving.canMove = false;
    }

    public void HidePath()
    {
        Destroy(pathMesh);
    }


    public virtual void ChangeLife(float delta, bool needUIJump = true)
    {
        LifeCurrent += delta;
        LifeCurrent = UnityEngine.Mathf.Clamp(LifeCurrent, 0, LifeAmount);
    }
    
    public void FaceTarget(UnityEngine.Transform target)
    {
        UnityEngine.Vector3 dir = target.transform.position - transform.position;
        FaceDir(dir);
    }

    public void FaceDir(UnityEngine.Vector3 v)
    {
        double angle = UnityEngine.Vector3.Angle(v, new UnityEngine.Vector3(1, 0, 0));
        if (angle < 90 || angle > 270)
        {
            transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        }
        else
        {
            transform.localEulerAngles = UnityEngine.Vector3.zero;
        }
        if (eye)
        {
            eye.dirCache = v;
        }        
    }

    public virtual void SpotByEnemy(Guard guard)
    {

    }

    public virtual void EnemyStopChasing(Guard guard)
    {

    }

    public virtual double GetSpeed()
    {
        return moving.speed;
    }

    public void Falling(int fallingDuration)
    {
        UnityEngine.Vector3 to = transform.position;
        UnityEngine.Vector3 from = transform.position + new UnityEngine.Vector3(0, 2000, 0);
        transform.position = from;
        AddAction(new MoveTo(transform, to, fallingDuration));
        Invoke("FallingOver", fallingDuration + 0.3f);
    }

    void FallingOver()
    {
        ClearAllActions();
    }
}