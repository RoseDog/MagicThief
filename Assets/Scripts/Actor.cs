public class Actor : UnityEngine.MonoBehaviour 
{
    [UnityEngine.HideInInspector]
    public UnityEngine.Animation anim;
    [UnityEngine.HideInInspector]
    public UnityEngine.CharacterController controller;

    public Action currentAction;

    [UnityEngine.HideInInspector]
    public Hitted hitted;
    [UnityEngine.HideInInspector]
    public LifeOver lifeOver;
    [UnityEngine.HideInInspector]
    public GuardMoving moving;

    [UnityEngine.HideInInspector]
    public UnityEngine.MeshRenderer[] meshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.SkinnedMeshRenderer[] skinnedMeshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.Renderer[] renderers;

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
    public int LifeCurrent;
    [UnityEngine.HideInInspector]
    public int PowerAmount;
    [UnityEngine.HideInInspector]
    public int PowerCurrent;
    

    public virtual void Awake()
    {        
        anim = GetComponent<UnityEngine.Animation>();
        controller = GetComponent<UnityEngine.CharacterController>();
        hitted = GetComponent<Hitted>();
        lifeOver = GetComponent<LifeOver>();
        moving = GetComponent<GuardMoving>();

        meshRenderers = GetComponentsInChildren<UnityEngine.MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<UnityEngine.SkinnedMeshRenderer>();
        renderers = GetComponentsInChildren<UnityEngine.Renderer>();

        pathMeshPrefab = UnityEngine.Resources.Load("Misc/PathMesh") as UnityEngine.GameObject;
    }

    public virtual void Start()
    {

    }

    // Action list
    private System.Collections.Generic.List<Cocos2dAction> actions = new System.Collections.Generic.List<Cocos2dAction>();

    // Update is called once per frame
    public virtual void FixedUpdate()
    {

        // Run actions
        UpdateActions();
    }

    // Update actions
    protected void UpdateActions()
    {

        // Run actions
        if (actions.Count > 0)
        {
            // Get current action instance
            Cocos2dAction action = actions[0];

            // Initialize action
            if (!action.IsInitialized()) action.Init();

            // Update action
            action.Update();

            // Remove action when completed
            if (action.IsCompleted()) actions.Remove(action);
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
        // Add action to list
        actions.Add(action);
        // Assign parent to action
        action.parent = this;
    }

    // Clear all Actions
    public void ClearAllActions()
    {
        actions.Clear();
    }

    public virtual void Visible(bool visibility)
    {
        foreach (UnityEngine.Renderer renderer in renderers)
        {
            renderer.enabled = visibility;
        }

        bVisible = visibility;
    }

    public bool IsLifeOver()
    {
        return currentAction == lifeOver;
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

    public bool MoveTo(UnityEngine.Vector3 pos, OnPathDelegate callback = null)
    {
        moving.canSearch = false;
        moving.GetSeeker().StartPath(moving.GetFeetPosition(), pos, callback);
        return true;
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
            if ((path[i].x == path[i - 1].x || path[i].z == path[i - 1].z) &&
                (path[i].x != path[i + 1].x && path[i].z != path[i + 1].z))
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
                if ((nextPoint.x < point.x && nextPoint.z < point.z)||
                    (nextPoint.x > point.x && nextPoint.z > point.z))
                {
                    flag = true;
                }
            }
            if (flag)
            {
                meshPoints.Add(new UnityEngine.Vector3(point.x + nodeSize * 0.25f, 0.5f, point.z - nodeSize * 0.25f));
                meshPoints.Add(new UnityEngine.Vector3(point.x - nodeSize * 0.25f, 0.5f, point.z + nodeSize * 0.25f));
            }
            else
            {
                meshPoints.Add(new UnityEngine.Vector3(point.x + nodeSize * 0.25f, 0.5f, point.z + nodeSize * 0.25f));
                meshPoints.Add(new UnityEngine.Vector3(point.x - nodeSize * 0.25f, 0.5f, point.z - nodeSize * 0.25f));
            }            
        }

        if (pathMesh == null)
        {
            pathMesh = UnityEngine.GameObject.Instantiate(pathMeshPrefab) as UnityEngine.GameObject;
            pathMesh.transform.position = new UnityEngine.Vector3(transform.position.x, 0.1f, transform.position.z);
        }
        pathMesh.GetComponent<PathMesh>().Construct(meshPoints);
    }

    public void ShowPathToPoint(UnityEngine.Vector3 destination)
    {
        MoveTo(destination, ShowPathComplete);
        moving.canMove = false;
    }

    public void HidePath()
    {
        Destroy(pathMesh);
    }

    public virtual bool ChangePower(int delta)
    {
        int powerTemp = PowerCurrent;
        powerTemp += delta;
        if (powerTemp < 0)
        {
            Globals.tipDisplay.Msg("魔力值不够了");
            return false;
        }
        else
        {
            PowerCurrent = powerTemp;
            return true;
        }
    }

    public virtual void ChangeLife(int delta)
    {
        LifeCurrent += delta;
        LifeCurrent = UnityEngine.Mathf.Clamp(LifeCurrent, 0, LifeAmount);
        if (LifeCurrent < UnityEngine.Mathf.Epsilon)
        {
            lifeOver.Excute();
        }
        else
        {
            hitted.Excute();
        }
    }

    public virtual void ResetLifeAndPower()
    {
        LifeCurrent = LifeAmount;
        PowerCurrent = PowerAmount;
    }
}