public class MyMazeBuilding : BuildingCouldDivedIn
{
    UnityEngine.RectTransform intro;
    UnityEngine.Vector3 introScaleCache;
    public override void Awake()
    {
        intro = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "intro");
        introScaleCache = intro.transform.localScale;
        intro.transform.localScale = UnityEngine.Vector3.zero;

        base.Awake();
    }

    public void IntoHomeMaze()
    {
        base.DivedIn();
        Globals.asyncLoad.ToLoadSceneAsync("MyMaze");        
    }

    public override void Choosen()
    {
        base.Choosen();
        AddAction(new Sequence(
            new ScaleTo(intro.transform, introScaleCache * 1.2f, Globals.uiMoveAndScaleDuration / 3),
            new ScaleTo(intro.transform, introScaleCache, Globals.uiMoveAndScaleDuration / 4)));

        Globals.languageTable.SetText(intro.GetComponentInChildren<MultiLanguageUIText>(), "my_maze_intro", new System.String[] { Globals.self.GetTotalIncomePerHour().ToString(), Globals.self.GetCashAmountOnMazeFloor().ToString() });        
    }

    public override void Unchoose()
    {
        AddAction(new ScaleTo(intro.transform, UnityEngine.Vector3.zero, Globals.uiMoveAndScaleDuration / 3));
        base.Unchoose();
    }

}