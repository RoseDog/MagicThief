public class TrickBtnOnGuardHead : CustomEventTrigger
{
    public Guard guard;    

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
    {
        base.OnPointerDown(data);
        if (Globals.magician.currentAction != Globals.magician.hypnosis)
        {
            Globals.magician.CastHypnosis(guard, btn);
        }        
    }
}
