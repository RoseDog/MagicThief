public class Machine : Guard
{
    TrickTimer fixTimer;
    public MachineActiveArea machineActiveArea;
    Cocos2dAction fixingOk = null;
    public override void Awake()
    {
        base.Awake();
        walkable = false;
        fixTimer = GetComponentInChildren<TrickTimer>();
        fixTimer.gameObject.SetActive(false);
        machineActiveArea = Globals.getChildGameObject(gameObject, "machineActiveArea").GetComponent<MachineActiveArea>();
        machineActiveArea.machine = this;
    }

    public override void Start()
    {
        base.Start();
        spriteSheet.Play("idle");
    }

    int fixingDuration = 700;
    public virtual void Broken()
    {        
        spriteSheet.enabled = false;                
        fixTimer.gameObject.SetActive(true);
        machineActiveArea.actorsInTouch.Clear();
        if (fixingOk == null)
        {
            fixTimer.BeginCountDown(this.gameObject, fixingDuration, UnityEngine.Vector3.zero);
            fixingOk = SleepThenCallFunction(fixingDuration, () => FixingComplete());
            machineActiveArea.gameObject.SetActive(false);
            machineActiveArea.characterController.enabled = false;
        }
        else
        {
            fixTimer.GetComponent<TrickTimer>().AddFrameTime(fixingDuration);
            RemoveAction(ref fixingOk);
            fixingOk = SleepThenCallFunction(fixTimer.GetComponent<TrickTimer>().GetLastFrameTime(), () => FixingComplete());
        }
    }

    public virtual void FixingComplete()
    {
        fixTimer.gameObject.SetActive(false);
        spriteSheet.enabled = true;
        machineActiveArea.gameObject.SetActive(true);
        machineActiveArea.characterController.enabled = true;
        fixingOk = null;
    }

    public virtual void EnterActiveArea(UnityEngine.GameObject other)
    {

    }
}
