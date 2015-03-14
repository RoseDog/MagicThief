public class Gem : Actor 
{
    FlyToScreenNumber fly;
    public int cashValue = 1000;
    
	public override void Awake () 
    {
        base.Awake();
        fly = GetComponent<FlyToScreenNumber>();
        
        int duration = UnityEngine.Random.Range(30, 50);
        UnityEngine.Vector3 pos_cache = transform.localPosition;
        AddAction(new RepeatForever(
            new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0, 0.1f, 0), duration/2),
            new MoveTo(transform, pos_cache, duration/2 )));
	}

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        int floatDuration = 60;

        Globals.Assert((Globals.LevelController as TutorialLevelController).unstolenGems.Contains(transform.parent.gameObject));
        (Globals.LevelController as TutorialLevelController).unstolenGems.Remove(transform.parent.gameObject);

        transform.parent = null;
        ClearAllActions();
        transform.localPosition = transform.localPosition - new UnityEngine.Vector3(0,0,0.6f);
        gameObject.layer = 26;
        AddAction(new MoveTo(transform, transform.localPosition + new UnityEngine.Vector3(0.0f, 2.0f, 0.0f), floatDuration));
        fly.numberDelta = cashValue;
        SleepThenCallFunction(floatDuration + 5, ()=>FlyOff());
    }

    void FlyOff()
    {
        fly.ToCashNumber(false);
    }
}
