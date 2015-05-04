public class Building : Actor 
{
    public City city;
    public BuildingData data;
    public override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<UnityEngine.SpriteRenderer>();
    }
	public virtual void Choosen()
    {

    }
    public virtual void Unchoose()
    {

    }
}
