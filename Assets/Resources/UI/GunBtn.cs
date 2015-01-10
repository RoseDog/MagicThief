[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class GunBtn : Actor
{
    public UnityEngine.GameObject tip;
    public UnityEngine.EventSystems.EventTrigger eventTrigger = null;
    public override void Awake()
    {
        eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        Globals.AddEventTrigger(eventTrigger, OnPointerEnter, UnityEngine.EventSystems.EventTriggerType.PointerEnter);
        Globals.AddEventTrigger(eventTrigger, OnPointerExit, UnityEngine.EventSystems.EventTriggerType.PointerExit);
        tip = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "ShotTip").gameObject;
        tip.SetActive(false);
        base.Awake();
    }
    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        tip.SetActive(true);
        tip.transform.localScale = UnityEngine.Vector3.zero;
        AddAction(new Sequence(
            new ScaleTo(tip.transform, new UnityEngine.Vector3(1.2f, 1.2f, 1.2f), Globals.uiMoveAndScaleDuration / 2),
            new ScaleTo(tip.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration / 4)));
    }

    private void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        tip.SetActive(false);   
    }
}
