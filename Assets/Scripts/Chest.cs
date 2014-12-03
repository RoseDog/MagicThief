public class Chest : Actor 
{
    bool isMagicianNear = false;
    bool isPlayingBack = false;
    GoldPoper goldPoper = null;
    public int goldAmount = 100;
    public int goldLostPersecond = 10;
    System.Collections.Generic.List<UnityEngine.Renderer> goldMeshes = new System.Collections.Generic.List<UnityEngine.Renderer>();
    UnityEngine.SkinnedMeshRenderer chestMesh;

    public override void Awake()
    {
        base.Awake();
        animation["ChestAnim"].speed = 2.0f;
        chestMesh = GetComponentInChildren<UnityEngine.SkinnedMeshRenderer>();
        foreach (UnityEngine.Renderer renderer in renderers)
        {
            if (renderer.tag == "GoldMesh")
            {
                goldMeshes.Add(renderer);
            }
        }
    }

	void Start () 
    {
        UnityEngine.GameObject effectPrefab = (UnityEngine.GameObject)UnityEngine.Resources.Load("Effects/GoldPoper/GoldPoper", typeof(UnityEngine.GameObject));
        goldPoper = (Instantiate(effectPrefab, transform.position + UnityEngine.Vector3.up * 0.5f, UnityEngine.Quaternion.identity) as UnityEngine.GameObject).GetComponent<GoldPoper>();
        goldPoper.InitParticleTex(goldLostPersecond);
        goldPoper.chest = this;
        goldPoper.transform.localScale = new UnityEngine.Vector3(2.0f, 2.0f, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        UnityEngine.Debug.Log("touch chest");
        isMagicianNear = true;
        if (goldAmount > 0)
        {
            animation.Play();
        }
        
        EnhanceEdge();
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        UnityEngine.Debug.Log("leave chest");
        isMagicianNear = false;
        if (goldAmount > 0 && animation["ChestAnim"].speed < UnityEngine.Mathf.Epsilon)
        {
            animation["ChestAnim"].speed = 2.0f;
        }

        goldPoper.StopPop();
        
        EdgeEnahancementOff();
    }

    public void ChestOpened()
    {
        isPlayingBack = true;
        UnityEngine.Debug.Log("ChestOpened");
        if (isMagicianNear)
        {
            animation["ChestAnim"].speed = 0.0f;
            goldPoper.Pop();
        }        
    }

    public void LostGold()
    {
        goldAmount -= goldLostPersecond;
        if (goldAmount <= 0)
        {
            foreach (UnityEngine.Renderer renderer in goldMeshes)
            {
                renderer.gameObject.SetActive(false);
            }
            goldPoper.StopPop();
        }
    }

    public void ChestClosed()
    {
        if (isPlayingBack)
        {
            UnityEngine.Debug.Log("ChestClosed");
            if (!isMagicianNear)
            {
                animation.Stop();
            }
        }
        isPlayingBack = false;
    }

    public void EnhanceEdge()
    {

        {
            chestMesh.material.shader = UnityEngine.Shader.Find("Custom/Rim");
            chestMesh.material.SetColor("_RimColor", UnityEngine.Color.yellow);            
        }

//         for (int idx = 0; idx < skinnedMeshRenderers.Length; ++idx)
//         {
//             skinnedMeshRenderers[idx].material.shader = UnityEngine.Shader.Find("Custom/ToonWithExtrusion");
//             skinnedMeshRenderers[idx].material.SetColor("_ExtrusionColor", UnityEngine.Color.yellow);
//             skinnedMeshRenderers[idx].material.SetFloat("_Amount", 0.1f);
//             skinnedMeshRenderers[idx].material.SetFloat("_Cutoff", 0.0f);
//         }
    }

    public void EdgeEnahancementOff()
    {
//         for (int idx = 0; idx < meshRenderers.Length; ++idx)
//         {
//             meshRenderers[idx].material.shader = UnityEngine.Shader.Find("Mobile/Unlit (Supports Lightmap)");
//         }
// 
//         for (int idx = 0; idx < skinnedMeshRenderers.Length; ++idx)
//         {
//             skinnedMeshRenderers[idx].material.shader = UnityEngine.Shader.Find("Mobile/Unlit (Supports Lightmap)");
//         }

        chestMesh.material.shader = UnityEngine.Shader.Find("Mobile/Unlit (Supports Lightmap)");
    }
}
