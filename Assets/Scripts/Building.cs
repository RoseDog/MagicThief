public class Building : Actor 
{
    public City city;
    public BuildingData data;

    public override void Start()
    {
        base.Start();
        UnityEngine.Sprite sprite = UnityEngine.Resources.Load<UnityEngine.Sprite>("Avatar/" + gameObject.name);
        if(sprite)
        {
            spriteRenderer.sprite = sprite;
        }        
    }

	public virtual void Choosen()
    {

    }

    public virtual void Unchoose()
    {

    }

    public System.String GetBuildingTimeLastStr(float seces)
    {
        System.String str = "";
        System.TimeSpan time = System.TimeSpan.FromSeconds(seces);

        if (time.Days != 0)
        {
            str += time.Days.ToString() + "D";
        }

        if (time.Hours != 0)
        {
            if (str != "")
            {
                str += " ";
            }
            str += time.Hours.ToString() + "H";
        }
        if (time.Minutes != 0)
        {
            if (str != "")
            {
                str += " ";
            }
            str += time.Minutes.ToString() + "M";
        }
        if (time.Seconds != 0)
        {
            if (str != "")
            {
                str += " ";
            }
            str += time.Seconds.ToString() + "S";
        }

        return str;
    }
}
