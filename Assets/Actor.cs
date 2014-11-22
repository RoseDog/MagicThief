using System.Collections;

public class Actor : UnityEngine.MonoBehaviour 
{
    public UnityEngine.Animation anim;
    public UnityEngine.CharacterController controller;
    public Action currentAction;
    public Hitted hitted;

    [UnityEngine.HideInInspector]
    public UnityEngine.MeshRenderer[] meshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.SkinnedMeshRenderer[] skinnedMeshRenderers;
    [UnityEngine.HideInInspector]
    public UnityEngine.Renderer[] renderers;

    public virtual void Awake()
    {
        anim = GetComponent<UnityEngine.Animation>();
        controller = GetComponent<UnityEngine.CharacterController>();
        hitted = GetComponent<Hitted>();

        meshRenderers = GetComponentsInChildren<UnityEngine.MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<UnityEngine.SkinnedMeshRenderer>();
        renderers = GetComponentsInChildren<UnityEngine.Renderer>();
    }

	// Use this for initialization
	public virtual void Start() 
    {
	
	}
}