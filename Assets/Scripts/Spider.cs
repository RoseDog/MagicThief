public class Spider : Machine 
{
    UnityEngine.GameObject appearSpot;
    public override void Awake()
    {
        base.Awake();
        spriteSheet.AddAnim("idle", 12);        
        spriteSheet.AddAnim("Atk", 4);
        spriteSheet.AddAnimationEvent("Atk", -1, () => atk.AtkEnd());
        spriteSheet.AddAnimationEvent("Atk", 1, () => atk.FireTheHit());

        appearSpot = Globals.getChildGameObject(gameObject, "appearSpot");
        appearSpot.SetActive(false);

        if(Globals.LevelController as StealingLevelController != null)
        {
            machineActiveArea.GetComponent<UnityEngine.MeshRenderer>().enabled = false;
        }        
    }

    public override void Start()
    {
        base.Start();
        // 界面显示会读取这个数字
        data.atkShortestDistance = (machineActiveArea.collider as UnityEngine.SphereCollider).radius * scaleCache.x;
    }

    public override void EnterActiveArea(UnityEngine.GameObject other)
    {
        base.EnterActiveArea(other);
        if (currentAction != atk)
        {
            if(appearSpot != null)
            {
                appearSpot.SetActive(true);
                AddAction(SleepThenCallFunction(5, () => DestroyAppearSpot())); 
            }            
            atk.Excute();
        }        
    }

    void DestroyAppearSpot()
    {
        Destroy(appearSpot);
        appearSpot = null;
    }

    public override void SetInFog(bool infog)
    {
        inFog = infog;
        machineActiveArea.GetComponent<UnityEngine.MeshRenderer>().enabled = !infog;
    }
}