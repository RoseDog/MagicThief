﻿
public class MagicThiefCamera : UnityEngine.MonoBehaviour
{
    public float dragSpeed = 0.3f;
    public UnityEngine.Vector3 lookAt;
    public UnityEngine.Vector3 disOffset;
    [UnityEngine.HideInInspector]
    public float disScale = 1.0f;

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

    public void SetDisScale(float scale)
    {
		disScale = UnityEngine.Mathf.Clamp(disScale + scale, 0.3f, 2.0f);
		dragSpeed = 0.05f * disScale;
    }

    public virtual void Update()
    {
        lookAt.x = UnityEngine.Mathf.Clamp(lookAt.x, Globals.map.WestPosInPixel(), Globals.map.EastPosInPixel());
        lookAt.z = UnityEngine.Mathf.Clamp(lookAt.z, Globals.map.SouthPosInPixel(), Globals.map.NorthPosInPixel());        
        transform.position = lookAt + disOffset * disScale;
        transform.LookAt(lookAt);
    }
}