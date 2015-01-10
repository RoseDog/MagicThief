[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class GuardBtn : UnityEngine.MonoBehaviour 
{
    public UnityEngine.GameObject guardSelectedImage;
    public UnityEngine.EventSystems.EventTrigger eventTrigger = null;
    public bool dragging = false;
    public bool inside = false;
    void Awake()
    {
        eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        Globals.AddEventTrigger(eventTrigger, OnBeginDrag, UnityEngine.EventSystems.EventTriggerType.BeginDrag);
        Globals.AddEventTrigger(eventTrigger, OnEndDrag, UnityEngine.EventSystems.EventTriggerType.EndDrag);
        Globals.AddEventTrigger(eventTrigger, OnDragging, UnityEngine.EventSystems.EventTriggerType.Drag);
        Globals.AddEventTrigger(eventTrigger, OnPointerEnter, UnityEngine.EventSystems.EventTriggerType.PointerEnter);
        Globals.AddEventTrigger(eventTrigger, OnPointerExit, UnityEngine.EventSystems.EventTriggerType.PointerExit);        
    }    

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        if (Globals.maze.draggingGuard != null)
        {
			UnityEngine.Debug.Log("Put Guard Back");
            Globals.DestroyGuard(Globals.maze.draggingGuard);
        }            
        inside = true;        
    }

    private void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        inside = false;
        if (dragging)
        {
            UnityEngine.Debug.Log("CreateGuard");
            Guard guard = null;
            UnityEngine.Vector3 screenPos = new UnityEngine.Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0);
            Pathfinding.Node birthNode = Globals.maze.GetNodeFromScreenRay(screenPos);
            if (birthNode != null)
            {                
                guard = Globals.CreateGuard(gameObject.name, birthNode);                
            }
            else
            {                                
                UnityEngine.RaycastHit hitInfo;
                int layermask = 1 << 9;
                UnityEngine.Ray ray = Globals.cameraFollowMagician.GetComponent<UnityEngine.Camera>().ScreenPointToRay(screenPos);
                if (UnityEngine.Physics.Raycast(ray, out hitInfo, 10000, layermask))
                {
                    guard = Globals.CreateGuard(gameObject.name, null);
                    guard.transform.position = hitInfo.point;
                }                
            }
           
            Globals.maze._DragGuard(guard);
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