[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class TrickBtnOnGuardHead : UnityEngine.MonoBehaviour 

{
    public Guard guard;
    UnityEngine.UI.Button btn;
    public UnityEngine.EventSystems.EventTrigger eventTrigger = null;
	// Use this for initialization
	void Awake () 
    {
        btn = GetComponent<UnityEngine.UI.Button>();
        eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        Globals.AddEventTrigger(eventTrigger, OnPointerDown, UnityEngine.EventSystems.EventTriggerType.PointerDown);
	}

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
    {
        if (Globals.magician.currentAction != Globals.magician.hypnosis)
        {
            Globals.magician.CastHypnosis(guard, btn);
        }        
    }
}
