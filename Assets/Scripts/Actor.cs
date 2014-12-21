public class Actor : UnityEngine.MonoBehaviour 
{
    public UnityEngine.Animation anim;
    public UnityEngine.CharacterController controller;
    public Action currentAction;
    public Hitted hitted;
    public LifeOver lifeOver;
    public GuardMoving moving;

    [UnityEngine.HideInInspector]
    public UnityEngine.MeshRenderer[] meshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.SkinnedMeshRenderer[] skinnedMeshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.Renderer[] renderers;

    public bool bVisible = false;

    public bool Stealing = false;

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

    public virtual void OnTargetReached()
    {
        
    }

}