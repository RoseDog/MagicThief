public class Machine : Guard
{
    TrickTimer fixTimer;
    public MachineActiveArea machineActiveArea;
    Cocos2dAction fixingOk = null;
    protected UnityEngine.Vector3 fixTimer_offset;
    public override void Awake()
    {
        base.Awake();
        walkable = false;
        fixTimer = GetComponentInChildren<TrickTimer>();
        fixTimer.gameObject.SetActive(false);
        machineActiveArea = Globals.getChildGameObject(gameObject, "machineActiveArea").GetComponent<MachineActiveArea>();
        machineActiveArea.machine = this;
        fixTimer_offset = UnityEngine.Vector3.zero;
    }

    public override void Start()
    {
        base.Start();
        spriteSheet.Play("idle");
    }

    public virtual void Broken(int fixDuration)
    {        
        spriteSheet.enabled = false;                
        fixTimer.gameObject.SetActive(true);
        machineActiveArea.actorsInTouch.Clear();
        if (fixingOk == null)
        {
            fixTimer.BeginCountDown(this.gameObject, fixDuration, fixTimer_offset);
            fixingOk = SleepThenCallFunction(fixDuration, () => FixingComplete());
            machineActiveArea.gameObject.SetActive(false);
            machineActiveArea.characterController.enabled = false;
        }
        else
        {
            fixTimer.GetComponent<TrickTimer>().AddFrameTime(fixDuration);
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
