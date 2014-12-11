public class FlyToScreenCashNumber : Actor
{
    // 这个是跟相机的projection matrix有关的。每次调整相机的距离可能需要调整这个参数
    public float cashDelta;
    public bool rotate = false;
    float scaleOnScreen = 0.015f;
    UnityEngine.Vector3 cashScreenPos;
    UnityEngine.Vector3 posWhenFlyBegin;
    UnityEngine.Vector3 scaleWhenFlyBegin;
    public void FloatUp()
    {
        // 钱图标的位置
        UnityEngine.RectTransform rect_transform = Globals.canvasForMagician.cashNumber.GetComponent<UnityEngine.RectTransform>();
        cashScreenPos = new UnityEngine.Vector3(rect_transform.position.x, rect_transform.position.y, Globals.cameraFollowMagician.camera.nearClipPlane);

        transform.parent = Globals.cameraFollowMagician.transform;
        posWhenFlyBegin = transform.localPosition;
        scaleWhenFlyBegin = transform.localScale;

        StartCoroutine(FlyToScreen());
        if (rotate)
        {
            UnityEngine.Vector3 from = new UnityEngine.Vector3(UnityEngine.Random.Range(0, 360),
                UnityEngine.Random.Range(0, 360), 
                UnityEngine.Random.Range(0, 360));
            UnityEngine.Vector3 to = new UnityEngine.Vector3(360.0f - from.x, 360.0f - from.z, 0.0f - from.z);
            
            AddAction(new RotateTo(from, to, UnityEngine.Random.Range(0.1f, 0.3f), true));
        }
    }

    System.Collections.IEnumerator FlyToScreen()
    {
        while (true)
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
                Globals.canvasForMagician.cashNumber.Add(cashDelta);
                DestroyImmediate(gameObject);
                Globals.LevelController.GotGem(cashDelta);
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
