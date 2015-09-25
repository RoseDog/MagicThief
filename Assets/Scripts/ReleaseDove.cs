public class ReleaseDove : MagicianTrickAction
{
    UnityEngine.GameObject dovePrefab;
    public override void Awake()
    {
        base.Awake();
        dovePrefab = UnityEngine.Resources.Load("Avatar/Dove") as UnityEngine.GameObject;        
    }

    public void Start()
    {
        mage.spriteSheet.AddAnimationEvent("dove_trick", 7, () => DoveFlyOut());
        mage.spriteSheet.AddAnimationEvent("dove_trick", -1, () => Stop());
    }

	public override void Excute()
    {
        base.Excute();
        mage.spriteSheet.Play("dove_trick");               
    }

    public void DoveFlyOut()
    {
        // 取最近的两个守卫
        System.Collections.Generic.List<Guard> guards = new System.Collections.Generic.List<Guard>(Globals.maze.guards.ToArray());
        guards.Sort();
        int dove_idx = 0;
        foreach (Guard guard in guards)
        {
            if (guard.moving == null)
            {
                continue;
            }
            //调用点不对？replay中创建之后是Active的，PvP中创建出来不是;
            //试一下延迟创建
            CreateDove(data, dovePrefab, guard.transform.position);
            ++dove_idx;
            if (dove_idx == 2)
            {
                break;
            }
        }

        while (dove_idx != 2)
        {
            CreateDove(data, dovePrefab, UnityEngine.Vector3.zero);
            ++dove_idx;
        }
    }

    void CreateDove(TrickData data, UnityEngine.GameObject dovePrefab, UnityEngine.Vector3 targetPos)
    {
        Dove dove = (UnityEngine.GameObject.Instantiate(dovePrefab) as UnityEngine.GameObject).GetComponent<Dove>();
        dove.transform.position = transform.position;
        if (targetPos == UnityEngine.Vector3.zero)
        {
            targetPos = dove.FindFarestWallDestination();
        }
        dove.StartOut(targetPos, data);
    }
}
