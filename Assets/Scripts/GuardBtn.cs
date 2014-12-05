﻿[System.Serializable]
public class Entry
{
    public UnityEngine.EventSystems.EventTrigger.TriggerEvent callback;
    public UnityEngine.EventSystems.EventTriggerType eventID;
    public Entry()
    {

    }
}
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class GuardBtn : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject guardSelectedImage;
    UnityEngine.EventSystems.EventTrigger eventTrigger = null;
    public bool dragging = false;
    public bool inside = false;
    void Awake()
    {
        eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        AddEventTrigger(OnBeginDrag, UnityEngine.EventSystems.EventTriggerType.BeginDrag);
        AddEventTrigger(OnEndDrag, UnityEngine.EventSystems.EventTriggerType.EndDrag);
        AddEventTrigger(OnDragging, UnityEngine.EventSystems.EventTriggerType.Drag);
        AddEventTrigger(OnPointerEnter, UnityEngine.EventSystems.EventTriggerType.PointerEnter);
        AddEventTrigger(OnPointerExit, UnityEngine.EventSystems.EventTriggerType.PointerExit);        
    }

    private void AddEventTrigger(UnityEngine.Events.UnityAction<UnityEngine.EventSystems.PointerEventData> action, UnityEngine.EventSystems.EventTriggerType triggerType)
    {
        // Create a nee TriggerEvent and add a listener
        UnityEngine.EventSystems.EventTrigger.TriggerEvent trigger = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
        UnityEngine.EventSystems.PointerEventData eventData =
            new UnityEngine.EventSystems.PointerEventData(UnityEngine.GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>());
        trigger.AddListener((data) => action(eventData)); // you can capture and pass the event data to the listener
        // Create and initialise EventTrigger.Entry using the created TriggerEvent
        UnityEngine.EventSystems.EventTrigger.Entry entry =
            new UnityEngine.EventSystems.EventTrigger.Entry() { callback = trigger, eventID = triggerType };
        // Add the EventTrigger.Entry to delegates list on the EventTrigger
        eventTrigger.delegates.Add(entry);
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        inside = true;        
    }

    private void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        inside = false;
        if (dragging)
        {
            UnityEngine.Debug.Log("CreateGuard");            
            Pathfinding.Node birthNode = Globals.map.GetNodeFromScreenRay(
                new UnityEngine.Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0));
            if (birthNode != null)
            {
                Guard guard = Globals.CreateGuard(gameObject.name, birthNode);
                guard.InitArrangeUI();
                Globals.map._DragGuard(guard);
            }
        }        
    }

    private void OnBeginDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().SetParent(GetComponent<UnityEngine.RectTransform>());
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector2.zero;
        guardSelectedImage.gameObject.SetActive(true);
        dragging = true;
    }

    private void OnEndDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        UnityEngine.Debug.Log("drag end");
        dragging = false;
        guardSelectedImage.gameObject.SetActive(false);
    }

    private void OnDragging(UnityEngine.EventSystems.PointerEventData data)
    {
        if (!inside)
        {            
        }
    }
}