public class RanksWindow : CustomEventTrigger 
{
    UnityEngine.GameObject playerRankPrefab;
    public System.Collections.Generic.List<CityEvent> records = new System.Collections.Generic.List<CityEvent>();
    public ViewRankPlayer viewRankPlayer;
	// Use this for initialization
	public override void Awake()
    {
        base.Awake();
        playerRankPrefab = UnityEngine.Resources.Load("UI/PlayerRank") as UnityEngine.GameObject;
	}
		
    public CityEvent AddRecord(PlayerInfo playerOnRank)
    {
        CityEvent record = (Instantiate(playerRankPrefab) as UnityEngine.GameObject).GetComponent<CityEvent>();
        UnityEngine.RectTransform ceTransform = record.GetComponent<UnityEngine.RectTransform>();
        ceTransform.SetParent(transform);
        ceTransform.localScale = new UnityEngine.Vector3(1, 1, 1);
        record.uiText.text = playerOnRank.name;
        UnityEngine.UI.Text RoseCount = Globals.getChildGameObject<UnityEngine.UI.Text>(record.gameObject, "RoseCount");
        RoseCount.text = playerOnRank.roseCount.ToString();
        records.Add(record);

        UnityEngine.UI.Button recordBtn = record.GetComponent<UnityEngine.UI.Button>();
        recordBtn.onClick.AddListener(() => ViewPlayerOnRank(playerOnRank));

        float event_y_pos = 136;
        float padding = 3;
        for (int idx = records.Count - 1; idx >= 0; --idx)
        {
            records[idx].rectTransform.localPosition = new UnityEngine.Vector3(0.0f, event_y_pos, 0.0f);
            event_y_pos -= records[idx].rectTransform.rect.height;
            event_y_pos -= padding;
        }

        return record;
    }

    public void ViewPlayerOnRank(PlayerInfo playerOnRank)
    {
        if (playerOnRank.atkReplays.Count == 0)
        {
            Globals.self.DownloadOtherReplays(playerOnRank);
        }
        else
        {
            viewRankPlayer.Open(playerOnRank);
        }        
    }

    public void OpenBtnClicked()
    {
        GetComponent<UIMover>().BeginMove(Globals.uiMoveAndScaleDuration);
    }

    public void CloseBtnClcked()
    {
        GetComponent<UIMover>().Goback(Globals.uiMoveAndScaleDuration);
        viewRankPlayer.OnTouchUpOutside(null);
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        //gameObject.SetActive(false);
    }
}
