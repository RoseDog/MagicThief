[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class CustomEventTrigger : Actor
{
    public UnityEngine.EventSystems.EventTrigger eventTrigger = null;
    public UnityEngine.UI.Button btn;
    public UnityEngine.RectTransform rectTransform;
    public bool dragging = false;
    public bool inside = false;

    public override void Awake()
    {
        base.Awake();
        eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        Globals.AddEventTrigger(eventTrigger, OnBeginDrag, UnityEngine.EventSystems.EventTriggerType.BeginDrag);
        Globals.AddEventTrigger(eventTrigger, OnEndDrag, UnityEngine.EventSystems.EventTriggerType.EndDrag);
        Globals.AddEventTrigger(eventTrigger, OnDragging, UnityEngine.EventSystems.EventTriggerType.Drag);
        Globals.AddEventTrigger(eventTrigger, OnPointerEnter, UnityEngine.EventSystems.EventTriggerType.PointerEnter);
        Globals.AddEventTrigger(eventTrigger, OnPointerExit, UnityEngine.EventSystems.EventTriggerType.PointerExit);
        Globals.AddEventTrigger(eventTrigger, OnPointerDown, UnityEngine.EventSystems.EventTriggerType.PointerDown);
        btn = GetComponent<UnityEngine.UI.Button>();
        rectTransform = GetComponent<UnityEngine.RectTransform>();        
    }

    public override void Update()
    {
        base.Update();
        Finger finger = Globals.input.GetFingerByID(0);
        if (finger.IsUp())
        {
            if (gameObject.activeSelf && !UnityEngine.RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            finger.nowPosition,
            null))
            {
                OnTouchUpOutside(finger);
            }
        }        
    }   

    public virtual void OnTouchUpOutside(Finger f)
    {
        
    }

    public virtual void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        inside = true;
    }

    public virtual void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        inside = false;       
    }

    public virtual void OnBeginDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        dragging = true;
    }

    public virtual void OnEndDrag(UnityEngine.EventSystems.PointerEventData data)
    {        
        dragging = false;
    }

    public virtual void OnDragging(UnityEngine.EventSystems.PointerEventData data)
    {
        if (!inside)
        {
        }
    }

    public virtual void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
    {
        
    }
}
