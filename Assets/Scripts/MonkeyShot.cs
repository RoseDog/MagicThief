public class MonkeyShot : Attack
{
    public override void AtkAnimation()
    {
        guard.FaceTarget(guard.spot.target);
        guard.spriteSheet.Play("Atk");
    }

    public override void FireTheHit()
    {
        UnityEngine.GameObject chickPrefab = UnityEngine.Resources.Load("Avatar/ChickenBullet") as UnityEngine.GameObject;
        UnityEngine.GameObject chick = UnityEngine.GameObject.Instantiate(chickPrefab) as UnityEngine.GameObject;
        chick.transform.position = transform.position;
        chick.GetComponent<ChickenBullet>().Fire(guard.spot.target.GetComponent<Actor>());
        chick.GetComponent<ChickenBullet>().monkey = guard;
    }
}
