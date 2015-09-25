public class Spider : Machine 
{
    UnityEngine.GameObject appearSpot;
    UnityEngine.GameObject scanner;
    public override void Awake()
    {
        base.Awake();
        spriteSheet.AddAnim("idle", 4, UnityEngine.Random.Range(0.5f,0.7f));        
        spriteSheet.AddAnim("Atk", 8, 1.7f);
        spriteSheet.AddAnimationEvent("Atk", -1, () => atk.AtkEnd());
        spriteSheet.AddAnimationEvent("Atk", 5, () => atk.FireTheHit());

        appearSpot = Globals.getChildGameObject(gameObject, "appearSpot");
        appearSpot.SetActive(false);

        scanner = Globals.getChildGameObject(machineActiveArea.gameObject, "scanner");
        AddAction(new RotateTo(new UnityEngine.Vector3(0, 0, 0), new UnityEngine.Vector3(0, 0, -360),
            UnityEngine.Random.Range(60, 90), true, scanner.transform));

        if(Globals.LevelController as StealingLevelController != null)
        {
            scanner.SetActive(false);
        }                        
    }

    public override void Start()
    {
        base.Start();
        // 界面显示会读取这个数字
        data.atkShortestDistance = machineActiveArea.characterController.radius * scaleCache.x;
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
        scanner.SetActive(!infog);
        machineActiveArea.GetComponent<UnityEngine.SpriteRenderer>().enabled = !infog;
        spriteRenderer.enabled = !infog;
    }

    public override void Broken(int fixDuration)
    {
        base.Broken(fixDuration);
        if(currentAction == atk)
        {
            atk.Stop();
            spriteSheet.Play("idle");
        }
    }
}