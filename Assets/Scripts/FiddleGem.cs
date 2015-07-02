public class FiddleGem : Actor 
{
    public override void Awake()
    {
        base.Awake();
        System.Collections.Generic.List<UnityEngine.Sprite> sprites = new System.Collections.Generic.List<UnityEngine.Sprite>();
        sprites.AddRange(UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Misc/juggle_gem"));
        sprites.AddRange(UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Misc/fiddle_gem"));
        spriteSheet._actor = this;
        spriteSheet._sprites = sprites.ToArray();
        spriteSheet.initialized = true;
        spriteSheet.AddAnim("juggle_gem", 4,0.7f);
        spriteSheet.AddAnim("fiddle_gem", 4, 0.5f);

        spriteSheet.AddAnimationEvent("juggle_gem", -1, () => SwitchToFiddle());
        spriteSheet.AddAnimationEvent("fiddle_gem", -1, () => SwitchToJuggle());
        spriteSheet.Play("fiddle_gem");
    }

    public void SwitchToFiddle()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.8f)
        {
            spriteSheet.Play("fiddle_gem");
        }
        else
        {
            spriteSheet.Play("juggle_gem");
        }
    }

    public void SwitchToJuggle()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.2f)
        {
            spriteSheet.Play("juggle_gem");
        }
        else
        {
            spriteSheet.Play("fiddle_gem");
        }
    }
}
