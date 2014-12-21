public class RoseBuilding : Building 
{
    UnityEngine.RectTransform peopleGivesYouRose;
    UnityEngine.RectTransform roseBtn;
    public override void Awake()
    {
        base.Awake();
        peopleGivesYouRose = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "PeopleGivesYouRose");
        peopleGivesYouRose.localScale = UnityEngine.Vector3.zero;
        roseBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "Rose");
        roseBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => RoseClicked());        
    }

    public override void Choosen()
    {
        if (peopleGivesYouRose.localScale.x < UnityEngine.Mathf.Epsilon)
        {
            AddAction(new Sequence(
            new ScaleTo(peopleGivesYouRose.transform, new UnityEngine.Vector3(1.2f, 1.2f, 1.2f), Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(peopleGivesYouRose.transform, new UnityEngine.Vector3(1.0f, 1.0f, 1.0f), Globals.uiMoveAndScaleDuration / 4)));
        }        
        base.Choosen();
    }

    public override void Unchoose()
    {
        AddAction(new ScaleTo(peopleGivesYouRose.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        base.Unchoose();
    }

    public void RoseClicked()
    {
        Choosen();
    }
}
