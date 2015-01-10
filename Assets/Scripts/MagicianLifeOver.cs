public class MagicianLifeOver : LifeOver
{
    public override void Excute()
    {
        UnityEngine.Debug.Log("magician life over");

        base.Excute();        
        actor.anim.Play("A_Stun_1");
        Invoke("Escape", actor.anim["A_Stun_1"].length);        
    }

    void Escape()
    {
        Globals.cameraFollowMagician.target = null;
        Globals.cameraFollowMagician.StaringMagician((actor as Magician).escape.duration - 0.8f);
        Globals.LevelController.Invoke("AfterMagicianLifeOverEscaped", (actor as Magician).escape.duration + 0.1f);
        (actor as Magician).escape.Excute();
    }
}
