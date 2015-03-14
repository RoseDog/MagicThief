public class RoseBuilding : Building 
{
    UnityEngine.RectTransform peopleGivesYouRose;
    UnityEngine.RectTransform roseBtn;
    UnityEngine.GameObject rose3d_prefab;
    UnityEngine.Vector3 tipScaleCache;
    public override void Awake()
    {
        base.Awake();
        peopleGivesYouRose = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "PeopleGivesYouRose");
        tipScaleCache = peopleGivesYouRose.transform.localScale;
        peopleGivesYouRose.localScale = UnityEngine.Vector3.zero;
        roseBtn = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "Rose");
        roseBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => RoseClicked());

        rose3d_prefab = UnityEngine.Resources.Load("Props/Rose3d") as UnityEngine.GameObject;
    }

    public override void Choosen()
    {
        if (peopleGivesYouRose.localScale.x < UnityEngine.Mathf.Epsilon)
        {
            AddAction(new Sequence(
            new ScaleTo(peopleGivesYouRose.transform, tipScaleCache*1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(peopleGivesYouRose.transform, tipScaleCache, Globals.uiMoveAndScaleDuration / 4)));
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
        UnityEngine.GameObject rose3d = UnityEngine.GameObject.Instantiate(rose3d_prefab) as UnityEngine.GameObject;
        rose3d.transform.position = roseBtn.transform.position;
        rose3d.GetComponent<FlyToScreenNumber>().ToRoseNumber();
        Globals.roseCount += 1;
    }
}
