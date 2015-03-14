public class GuardBtn : CustomEventTrigger
{
    public UnityEngine.GameObject guardSelectedImage;
            
    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnPointerEnter(data);
        if (Globals.maze.draggingGuard != null)
        {
			UnityEngine.Debug.Log("Put Guard Back");
            Globals.DestroyGuard(Globals.maze.draggingGuard);
        }            
    }

    public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnPointerExit(data);
        if (dragging)
        {
            UnityEngine.Debug.Log("CreateGuard");
            Guard guard = null;
            UnityEngine.Vector3 screenPos = new UnityEngine.Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 0);
            Pathfinding.Node birthNode = Globals.maze.GetNodeFromScreenRay(screenPos);
            guard = Globals.CreateGuard(Globals.GetGuardData(gameObject.name), birthNode);            
           
            Globals.maze._DragGuard(guard);
        }        
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnBeginDrag(data);
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().SetParent(GetComponent<UnityEngine.RectTransform>());
        guardSelectedImage.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector2.zero;
        guardSelectedImage.gameObject.SetActive(true);
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnEndDrag(data);
        guardSelectedImage.gameObject.SetActive(false);
    }
}