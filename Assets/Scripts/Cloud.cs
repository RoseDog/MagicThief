using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour 
{
    CloudData data;
    public UnityEngine.UI.Text cost;
    public UnityEngine.GameObject view;
    public UnityEngine.Vector3 viewScaleCache;

    public void Awake()
    {
        viewScaleCache = view.transform.localScale;
    }

    public void Clicked()
    {
        MsgBox msgbox = Globals.MessageBox("", () => ClickToUnlockCloud(), true);
        Globals.languageTable.SetText(msgbox.msgBoxText, "sure_to_unlock_cloud", new System.String[] { data.price.ToString() });
    }

    public void SyncData(CloudData d)
    {
        data = d;
        cost.text = ((int)d.price).ToString();

        Building[] buildings = GetComponentsInChildren<Building>();
        foreach (Building b in buildings)
        {
            if (data.locked)
            {
                b.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                b.transform.parent.parent = gameObject.transform.parent;
            }
        }

        if (data.locked)
        {
            view.gameObject.SetActive(false);
            view.transform.localScale = UnityEngine.Vector3.zero;
        }
        else
        {
            view.transform.parent = gameObject.transform.parent;
            view.SetActive(true);
        }

        gameObject.SetActive(data.locked);
    }

    public void ClickToUnlockCloud()
    {
        if (Globals.canvasForMagician.ChangeCash(-data.price))
        {
            data.locked = false;
            Globals.self.CloudUnlock(data);
            
            Building[] buildings = GetComponentsInChildren<Building>(true);
            foreach(Building b in buildings)
            {
                b.transform.parent.gameObject.SetActive(true);
                b.transform.parent.parent = gameObject.transform.parent;
                if (b.data.type == "locked")
                {
                    Globals.self.BuildingUnlock(b.data);
                }                
            }
            view.transform.parent = gameObject.transform.parent;
            Globals.city.AddAction(new ScaleTo(view.transform, viewScaleCache, 50));
            view.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
