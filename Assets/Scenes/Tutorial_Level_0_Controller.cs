public class Tutorial_Level_0_Controller : TutorialLevelController
{
    UnityEngine.Vector3 posOnSky = new UnityEngine.Vector3(0.0f, 8.0f, 0.0f);
    float fallingTime = 1.5f;
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

        // 主角降下
        Globals.magician.AddAction(new MoveTo(Globals.magician.transform.position, fallingTime));
        UnityEngine.Camera.main.transform.position = Globals.magician.transform.position + Globals.magician.transform.forward * 3.0f;
        Globals.magician.transform.position += posOnSky;
        Globals.magician.anim.Play("A_Falling_1");
        Globals.magician.isInAir = true;
        // 禁止输入        
        Globals.EnableAllInput(false);                
        StartCoroutine(CamLookAtMagicianDuringFalling());
    }

    System.Collections.IEnumerator CamLookAtMagicianDuringFalling()
    {
        float time = fallingTime;
        while (time > 0.0f)
        {
            time = time - UnityEngine.Time.deltaTime;
            Globals.cameraFollowMagician.transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));
            yield return null;
        }
        Globals.magician.anim.Play("A_JumpLanding_1");
        Invoke("LandingOver", 1.5f);
        yield return null;
    }

    void LandingOver()
    {
        Globals.transition.BlackOut(this, "StealingBegin");
    }
    

    public override void GotGem(int value)
    {
        base.GotGem(value);
        Gem[] gems = UnityEngine.GameObject.FindObjectsOfType<Gem>();
        if (gems.Length == 0)
        {
            Invoke("LevelPassed",0.5f);
        }
    }

    void LevelPassed()
    {
        Globals.asyncLoad.ToLoadSceneAsync("Tutorial_Level_1");
    }	
}
