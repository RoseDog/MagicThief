public class Tutorial_Level_0_Controller : TutorialLevelController
{
    UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 8.0f, 0.0f);
    UnityEngine.Vector3 cameraOffsetCache;
    float fallingDuration = 1.5f;
	// Use this for initialization
    public override void Awake()
    {
        base.Awake();        
    }

    public override void Start()
    {
        base.Start();        
    }

    public override void MazeFinished()
    {
        base.MazeFinished();
        Globals.canvasForMagician.SetLifeVisible(false);

        // 魔术师出场
        UnityEngine.GameObject magician_prefab = UnityEngine.Resources.Load("Avatar/Mage_Girl") as UnityEngine.GameObject;
        UnityEngine.GameObject magician = UnityEngine.GameObject.Instantiate(magician_prefab) as UnityEngine.GameObject;

        Cell magician_birth = Globals.map.entryOfMaze;
        magician.name = "Mage_Girl";
        magician.transform.position = magician_birth.GetFloorPos();

        // 相机就位
        Globals.cameraFollowMagician.transform.position = Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f;
        Globals.cameraFollowMagician.enabled = false;
        StartCoroutine(CamLookAtMagicianDuringFalling());

        // 主角降下            
        Globals.magician.Falling(Globals.magician.transform.position + posOnSky, 
            Globals.magician.transform.position,
            fallingDuration);
        // 禁止输入        
        Globals.EnableAllInput(false);        
    }

    System.Collections.IEnumerator CamLookAtMagicianDuringFalling()
    {
        float time = fallingDuration;
        while (time > 0.0f)
        {
            time = time - UnityEngine.Time.deltaTime;
            Globals.cameraFollowMagician.transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));
            yield return null;
        }        
        Invoke("LandingOver", 1.5f);
        yield return null;
    }

    void LandingOver()
    {
        Globals.transition.BlackOut(this, "StealingBegin");
    }

    public override void StealingBegin()
    {
        base.StealingBegin();
        Globals.cameraFollowMagician.enabled = true;
        Globals.map.RegistChallengerEvent();
    }

    public override void LevelPassed()
    {
        base.LevelPassed();
        Globals.asyncLoad.ToLoadSceneAsync("City");
    }	
}
