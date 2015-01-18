public class Lamp : Guard 
{
    TrickTimer fixTimer;
    UnityEngine.GameObject bulb;
    UnityEngine.GameObject LightCone;
    public override void Awake()
    {
        base.Awake();
        walkable = false;
        fixTimer = GetComponentInChildren<TrickTimer>();
        fixTimer.gameObject.SetActive(false);
        bulb = Globals.getChildGameObject(gameObject, "bulb");
        LightCone = Globals.getChildGameObject(gameObject, "LightCone");
    }

    float fixingDuration = 15.0f;
    public void BulbBroken()
    {
        fixTimer.BeginCountDown(bulb, fixingDuration, UnityEngine.Vector3.zero);
        bulb.SetActive(false);
        LightCone.SetActive(false);
        fixTimer.gameObject.SetActive(true);
        Invoke("FixingComplete", fixingDuration);
    }

    public void FixingComplete()
    {
        fixTimer.gameObject.SetActive(false);
        bulb.SetActive(true);
        LightCone.SetActive(true);
    }
}
