public class Victory : Action
{
    public override void Excute()
    {        
        base.Excute();
        UnityEngine.Debug.Log("magician victory:" + actor.anim["A_Shake_Hand_1"].length.ToString());
        actor.OutStealing();
        Invoke("Escape",actor.anim["A_Shake_Hand_1"].length);
        actor.anim.Play("A_Shake_Hand_1");        
    }

    void Escape()
    {
        Globals.cameraFollowMagician.StaringMagician((actor as Magician).escape.duration - 0.5f);
        Globals.LevelController.Invoke("AfterMagicianSuccessedEscaped", (actor as Magician).escape.duration + 0.1f);
        (actor as Magician).escape.Excute();        
    }
}
