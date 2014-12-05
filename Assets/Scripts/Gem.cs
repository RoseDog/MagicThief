public class Gem : Actor 
{
    UnityEngine.Animator animator;
    // 这个是跟相机的projection matrix有关的。每次调整相机的距离可能需要调整这个参数
    float scaleOnScreen = 0.015f;
    UnityEngine.Vector3 cashScreenPos;
    UnityEngine.Vector3 posWhenFlyBegin;
    UnityEngine.Vector3 scaleWhenFlyBegin;
    public int cashValue = 1000;
    
	public override void Awake () 
    {
        base.Awake();
        animator = GetComponent<UnityEngine.Animator>();
        animator.speed = UnityEngine.Random.Range(0.7f, 1.4f);
        animator.Play("autoRotationForever");        
	}

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        animator.enabled = false;
        float floatDuration = 0.8f;
        transform.parent = null;
        AddAction(new MoveTo(transform.localPosition + new UnityEngine.Vector3(0.0f, 1.0f, 0.0f), floatDuration));
        Invoke("FloatUp", floatDuration+0.1f);
    }

    void FloatUp()
    {        
        // 钱图标的位置
        
        UnityEngine.RectTransform rect_transform = Globals.canvasForMagician.cashNumber.GetComponent<UnityEngine.RectTransform>();
        cashScreenPos = new UnityEngine.Vector3(rect_transform.position.x, rect_transform.position.y, Globals.cameraFollowMagician.camera.nearClipPlane);

        transform.parent = Globals.cameraFollowMagician.transform;        
        posWhenFlyBegin = transform.localPosition;
        scaleWhenFlyBegin = transform.localScale;
        
        StartCoroutine(FlyToScreen());
    }

    System.Collections.IEnumerator FlyToScreen()
    {
        while(true)
        {
            UnityEngine.Vector3 destination = Globals.cameraFollowMagician.camera.ScreenToWorldPoint(cashScreenPos) - Globals.cameraFollowMagician.transform.position;
            destination.y = -destination.y;
            float disNow = UnityEngine.Vector3.Distance(destination, transform.localPosition);
            float dis = UnityEngine.Vector3.Distance(destination, posWhenFlyBegin);
            float disRatio = disNow / dis;

            float scaleNow = scaleOnScreen + (1.0f - scaleOnScreen) * disRatio;
            transform.localScale = scaleWhenFlyBegin * scaleNow;

            transform.localPosition += (destination - transform.localPosition) * 0.2f;

            if (disNow < 0.01f)
            {
                Globals.canvasForMagician.cashNumber.Add(cashValue);             
                DestroyImmediate(gameObject);                
                Globals.pveLevelController.GotGem(cashValue);
                break;
            }
            else
            {
                yield return null;
            }
        }
        yield return null;
    }
}
