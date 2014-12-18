public class LevelTipBillboard : Actor
{
    UnityEngine.RectTransform LevelTipCanvas;
    void Start()
    {
        LevelTipCanvas = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "LevelTipCanvas");
        LevelTipCanvas.transform.localScale = UnityEngine.Vector3.zero;
    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {
        LevelTipCanvas.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new ScaleTo(LevelTipCanvas.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration));
    }

    void OnTriggerExit(UnityEngine.Collider other)
    {
        LevelTipCanvas.transform.localScale = new UnityEngine.Vector3(1,1,1);
        AddAction(new ScaleTo(LevelTipCanvas.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration));
    }
}
