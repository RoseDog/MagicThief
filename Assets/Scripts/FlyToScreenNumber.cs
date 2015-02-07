public class FlyToScreenNumber : Actor
{
    // 这个是跟相机的projection matrix有关的。每次调整相机的距离可能需要调整这个参数
    public float numberDelta;
    float scaleOnScreen = 0.015f;
    UnityEngine.Vector3 numberScreenPos;
    UnityEngine.Vector3 posWhenFlyBegin;
    UnityEngine.Vector3 scaleWhenFlyBegin;
    Number numberFlyTo;

    public void ToCashNumber(bool rotate)
    {
        // 在教程中，TutorialThief偷东西的时候，不往界面上飞
        TutorialLevelController controller = Globals.LevelController as TutorialLevelController;

        // 金钱增量的位置
        numberFlyTo = controller.StealingCash;
        FlyOff(rotate);
    }

    public void ToRoseNumber()
    {        
        // 玫瑰数字的位置
        scaleOnScreen = 0.002f;
        numberFlyTo = Globals.canvasForMagician.RoseNumber;
        numberDelta = 1;
        FlyOff(false);
    }

    public void FlyOff(bool rotate)
    {
        UnityEngine.RectTransform rect_transform = numberFlyTo.GetComponent<UnityEngine.RectTransform>();
        numberScreenPos = new UnityEngine.Vector3(rect_transform.position.x, rect_transform.position.y, Globals.cameraFollowMagician.camera.nearClipPlane);

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
            UnityEngine.Vector3 uiWorldPos = Globals.cameraFollowMagician.camera.ScreenToWorldPoint(numberScreenPos);
            UnityEngine.Vector3 destination = Globals.cameraFollowMagician.transform.InverseTransformPoint(uiWorldPos);            
            
            float disNow = UnityEngine.Vector3.Distance(destination, transform.localPosition);
            float dis = UnityEngine.Vector3.Distance(destination, posWhenFlyBegin);
            float disRatio = disNow / dis;

            float scaleNow = scaleOnScreen + (1.0f - scaleOnScreen) * disRatio;
            transform.localScale = scaleWhenFlyBegin * scaleNow;

            transform.localPosition += (destination - transform.localPosition) * 0.2f;

            if (disNow < 0.01f)
            {
                DestroyImmediate(gameObject);
                if (Globals.magician.LifeCurrent > 0)
                {
                    numberFlyTo.Add(numberDelta);
                    Globals.LevelController.MagicianGotCash(numberDelta);
                }
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
