public class TargetBuilding : Building
{    
    public UnityEngine.UI.Text tip;
    public override void Awake()
    {        
        tip = Globals.getChildGameObject<UnityEngine.UI.Text>(gameObject, "Tip");        
        base.Awake();
    }

    public override void Choosen()
    {
        base.Choosen();
        if (tip)
        {
            city.TargetClicked(tip.text);
        }        
    }

    float fallingDuration = 0.8f;
    public override void DivedIn()
    {
        base.DivedIn();        

        UnityEngine.BoxCollider box_collider = collider as UnityEngine.BoxCollider;
        UnityEngine.Vector3 landing_pos = new UnityEngine.Vector3(
            transform.position.x + box_collider.center.x * transform.localScale.x, 
            transform.position.y + box_collider.size.y * transform.localScale.y * 0.5f,
            transform.position.z + box_collider.center.z * transform.localScale.z);

        UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
        UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;
        Globals.magician.falling.from = landing_pos + new UnityEngine.Vector3(0.0f, 20.0f, 0.0f);
        Globals.magician.falling.to = landing_pos;
        Globals.magician.falling.duration = fallingDuration;
        Globals.magician.falling.Excute();        
        magician.transform.Rotate(new UnityEngine.Vector3(0, 1, 0), 180.0f);
        Globals.EnableAllInput(false);

        Invoke("InToBuilding", fallingDuration + 0.5f);        
    }

    void InToBuilding()
    {        
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Levels");
    }
}
