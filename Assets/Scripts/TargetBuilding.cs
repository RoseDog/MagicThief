public class TargetBuilding : Actor
{
    UnityEngine.GameObject tip;
    UnityEngine.GameObject dive_in_btn;
    void Awake()
    {
        tip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "Tip").gameObject;
        dive_in_btn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "DiveIn").gameObject;
        dive_in_btn.SetActive(false);
    }

    public void Choosen()
    {        
        dive_in_btn.gameObject.SetActive(true);
        dive_in_btn.transform.localScale = UnityEngine.Vector3.zero;
        
        AddAction(new Cocos2dParallel(
            new ScaleTo(tip.transform, UnityEngine.Vector3.zero, 0.3f ),
            new ScaleTo(dive_in_btn.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), 0.3f)));
    }

    public void Unchoose()
    {
    }

    float fallingDuration = 0.8f;
    public void DivedIn()
    {        
        AddAction(new ScaleTo(dive_in_btn.transform, UnityEngine.Vector3.zero, 0.3f));
        UnityEngine.BoxCollider box_collider = collider as UnityEngine.BoxCollider;
        UnityEngine.Vector3 landing_pos = new UnityEngine.Vector3(
            transform.position.x + box_collider.center.x * transform.localScale.x, 
            transform.position.y + box_collider.size.y * transform.localScale.y * 0.5f,
            transform.position.z + box_collider.center.z * transform.localScale.z);

        UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
        UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;
        Globals.magician.Falling(landing_pos + new UnityEngine.Vector3(0.0f, 20.0f, 0.0f),
            landing_pos,
            fallingDuration);
        magician.transform.Rotate(new UnityEngine.Vector3(0, 1, 0), 180.0f);
        Globals.EnableAllInput(false);

        Invoke("InToBuilding", fallingDuration + 0.5f);        
    }

    void InToBuilding()
    {
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Level_1");
    }
}
