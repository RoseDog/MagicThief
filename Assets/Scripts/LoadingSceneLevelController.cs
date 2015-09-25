public class LoadingSceneLevelController : LevelController 
{
    public UnityEngine.UI.Image progress;
    public MouseHoverSpriter rosa;
    public MouseHoverSpriter police;
    public MouseHoverSpriter hippo;
    public MouseHoverSpriter joker;
    public MouseHoverSpriter dog;

    public override void Awake()
    {
        Globals.loadingLevelController = this;
    }

    public override void Start()
    {
        rosa.spriteSheet.frameIdx = UnityEngine.Random.Range(0, rosa.spriteSheet._animationList["play"].spriteList.Count);
        police.spriteSheet.frameIdx = UnityEngine.Random.Range(0, police.spriteSheet._animationList["play"].spriteList.Count);
        hippo.spriteSheet.frameIdx = UnityEngine.Random.Range(0, hippo.spriteSheet._animationList["play"].spriteList.Count);
        joker.spriteSheet.frameIdx = UnityEngine.Random.Range(0, joker.spriteSheet._animationList["play"].spriteList.Count);
        dog.spriteSheet.frameIdx = UnityEngine.Random.Range(0, dog.spriteSheet._animationList["play"].spriteList.Count);
    }

    public override void OnDestroy()
    {
        Globals.loadingLevelController = null;
    }
}
