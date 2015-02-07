
public class MagicThiefCamera : UnityEngine.MonoBehaviour
{
    float dragCamSpeed = 0.02f;
    public UnityEngine.Vector3 lookAt;
    public UnityEngine.Vector3 disOffset;
    public UnityEngine.Vector3 lookAtCache;
    public UnityEngine.Vector3 disOffsetCache;
    [UnityEngine.HideInInspector]
    public float disScale = 1.0f;

    public UnityEngine.Vector2 restriction_x = new UnityEngine.Vector2(-5, 5);
    public UnityEngine.Vector2 restriction_z = new UnityEngine.Vector2(-5, 5);

    public UnityEngine.Transform target;            

    public virtual void Awake()
    {
        lookAtCache = lookAt;
        disOffsetCache = disOffset;
        Globals.cameraFollowMagician = this;
    }

    public void Reset()
    {
        enabled = true;
        lookAt = lookAtCache;
        disOffset = disOffsetCache;
        SetDragSpeed(0.06f);
    }

    public void SetDragSpeed(float speed)
    {
        dragCamSpeed = speed;
    }
    
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
		//dragCamSpeed = 0.05f * disScale;
    }

    UnityEngine.Vector3 EndOffset;
    UnityEngine.Vector3 StartOffset;
    UnityEngine.Vector3 EndLookAt;
    UnityEngine.Vector3 StartMove;
    float startTime;
    float durationTime;
    float currentTime;
    public void MoveToPoint(UnityEngine.Vector3 look, UnityEngine.Vector3 offset, float duration)
    {
        enabled = true;
        EndLookAt = RestrictPosition(look);
        EndOffset = offset;
        StartMove = lookAt;
        StartOffset = disOffset;
        startTime = UnityEngine.Time.time;
        durationTime = duration;
        currentTime = durationTime;
        //moving = true;
        StartCoroutine(Moving());
    }

    //bool moving = false;
    System.Collections.IEnumerator Moving()
    {
        while (currentTime > 0)
        {
            float time = UnityEngine.Time.time - startTime;
            
            time -= 1;
            float temp = time * time * time * time * time + 1;
            
            lookAt = UnityEngine.Vector3.Lerp(StartMove, EndLookAt, temp);
            disOffset = UnityEngine.Vector3.Lerp(StartOffset, EndOffset, temp);

            currentTime -= UnityEngine.Time.deltaTime;
            yield return null;
        }
        lookAt = EndLookAt;
        disOffset = EndOffset;
        UnityEngine.Debug.Log(disOffset);
        //moving = false;
    }

    bool bStaring = false;
    public void StaringMagician(float duration)
    {
        bStaring = true;
        Invoke("EndStaring", duration);
    }

    void EndStaring()
    {
        UnityEngine.Debug.Log("EndStaring");
        bStaring = false;
        enabled = false;
    }

    public void DragToMove(Finger finger)
    {
		// two fingers touch , drag camaera not allowed
		Finger finger0 = Globals.input.GetFingerByID(0);
		Finger finger1 = Globals.input.GetFingerByID(1);
		if (!(finger0.enabled && finger1.enabled))
		{
			UnityEngine.Vector2 finger_move_delta = finger.MovmentDelta();
			UnityEngine.Vector3 cameraHorForward = GetHorForward();
			UnityEngine.Vector3 cameraHorRight = GetHorRight();
			UnityEngine.Vector3 movementDirection = -cameraHorForward * finger_move_delta.y - cameraHorRight * finger_move_delta.x;
			lookAt += movementDirection * dragCamSpeed;
			lookAt = RestrictPosition(lookAt);
		}
    }

    public virtual void Update()
    {        
        if (bStaring)
        {
            transform.LookAt(Globals.magician.transform.position + new UnityEngine.Vector3(0.0f, 0.5f, 0.0f));           
        }
        else
        {
            if (target != null)
            {
                lookAt = target.position;
                lookAt = RestrictPosition(lookAt);
            }
            transform.position = lookAt + disOffset * disScale;
            transform.LookAt(lookAt);
        }        
    }

    public UnityEngine.Vector3 RestrictPosition(UnityEngine.Vector3 pos)
    {
        pos.x = UnityEngine.Mathf.Clamp(pos.x, restriction_x.x, restriction_x.y);
        pos.z = UnityEngine.Mathf.Clamp(pos.z, restriction_z.x, restriction_z.y);
        return pos;
    }
}
