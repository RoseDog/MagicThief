
public class MagicThiefCamera : UnityEngine.MonoBehaviour
{
    public float dragSpeed = 0.3f;
    public UnityEngine.Vector3 lookAt;
    public UnityEngine.Vector3 disOffset;
    [UnityEngine.HideInInspector]
    public float disScale = 1.0f;

    public UnityEngine.Vector2 restriction_x = new UnityEngine.Vector2(-5, 5);
    public UnityEngine.Vector2 restriction_z = new UnityEngine.Vector2(-5, 5);

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

    UnityEngine.Vector3 EndOffset;
    UnityEngine.Vector3 StartOffset;
    UnityEngine.Vector3 EndLookAt;
    UnityEngine.Vector3 StartMove;
    float startTime;
    float durationTime;
    public void MoveToPoint(UnityEngine.Vector3 look, UnityEngine.Vector3 offset, float duration)
    {
        EndLookAt = RestrictPosition(look);
        EndOffset = offset;
        StartMove = lookAt;
        StartOffset = disOffset;
        startTime = UnityEngine.Time.time;
        durationTime = duration;
        moving = true;
        StartCoroutine(Moving());
    }

    bool moving = false;
    System.Collections.IEnumerator Moving()
    {
        while (moving && !Globals.Vector3AlmostEqual(lookAt ,EndLookAt, 0.1f))
        {
            lookAt = UnityEngine.Vector3.Lerp(StartMove, EndLookAt,(UnityEngine.Time.time - startTime) / durationTime);
            disOffset = UnityEngine.Vector3.Lerp(StartOffset, EndOffset, (UnityEngine.Time.time - startTime) / durationTime);
            yield return null;
        }
        lookAt = EndLookAt;
        disOffset = EndOffset;
        moving = false;
    }

    public void DragToMove(Finger finger)
    {
        UnityEngine.Vector2 finger_move_delta = finger.MovmentDelta();
        UnityEngine.Vector3 cameraHorForward = GetHorForward();
        UnityEngine.Vector3 cameraHorRight = GetHorRight();
        UnityEngine.Vector3 movementDirection = -cameraHorForward * finger_move_delta.y - cameraHorRight * finger_move_delta.x;
        lookAt += movementDirection * dragSpeed;      
    }

    public virtual void Update()
    {
        lookAt = RestrictPosition(lookAt);
        transform.position = lookAt + disOffset * disScale;
        transform.LookAt(lookAt);                
    }

    UnityEngine.Vector3 RestrictPosition(UnityEngine.Vector3 pos)
    {
        if (!moving)
        {
            pos.x = UnityEngine.Mathf.Clamp(pos.x, restriction_x.x, restriction_x.y);
            pos.z = UnityEngine.Mathf.Clamp(pos.z, restriction_z.x, restriction_z.y);
        }
        return pos;
    }
}
