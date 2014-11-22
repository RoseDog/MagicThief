
public class MagicThiefCamera : UnityEngine.MonoBehaviour
{
    public float dragSpeed = 0.3f;
    public UnityEngine.Vector3 lookAt;
    public UnityEngine.Vector3 disOffset;

    public UnityEngine.Vector3 GetHorForward()
    {
        UnityEngine.Vector3 cameraHorForward = transform.forward;
        cameraHorForward.y = 0.0f;
        return cameraHorForward.normalized;
    }

    public UnityEngine.Vector3 GetHorRight()
    {
        UnityEngine.Vector3 cameraHorRight = transform.right;
        cameraHorRight.y = 0.0f;
        return cameraHorRight.normalized;
    }

    public virtual void Update()
    {
        lookAt.x = UnityEngine.Mathf.Clamp(lookAt.x, Globals.map.WestPosInPixel(), Globals.map.EastPosInPixel());
        lookAt.z = UnityEngine.Mathf.Clamp(lookAt.z, Globals.map.SouthPosInPixel(), Globals.map.NorthPosInPixel());        
        transform.position = lookAt + disOffset;
        transform.LookAt(lookAt);
    }
}
