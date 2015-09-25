public class RanksWindow : CustomEventTrigger 
{
    UnityEngine.GameObject playerRankPrefab;
    public System.Collections.Generic.List<CityEvent> records = new System.Collections.Generic.List<CityEvent>();
    public ViewRankPlayer viewRankPlayer;
    public UnityEngine.GameObject highLightFrame;
	// Use this for initialization
	public override void Awake()
    {
        base.Awake();
        playerRankPrefab = UnityEngine.Resources.Load("UI/PlayerRank") as UnityEngine.GameObject;
        gameObject.SetActive(false);

        highLightFrame = Globals.getChildGameObject<UnityEngine.RectTransform>(gameObject, "highLightFrame").gameObject;
        highLightFrame.SetActive(false);
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
        recordBtn.onClick.AddListener(() => ViewPlayerOnRank(recordBtn,playerOnRank));

        float event_y_pos = 122;
        float padding = 3;
        for (int idx = records.Count - 1; idx >= 0; --idx)
        {
            records[idx].rectTransform.localPosition = new UnityEngine.Vector3(10.0f, event_y_pos, 0.0f);
            event_y_pos -= records[idx].rectTransform.rect.height;
            event_y_pos -= padding;
        }

        

        return record;
    }

    public void ViewPlayerOnRank(UnityEngine.UI.Button recordBtn, PlayerInfo playerOnRank)
    {
        if (playerOnRank.atkReplays.Count == 0)
        {
            Globals.self.DownloadOtherReplays(playerOnRank);
        }
        else
        {
            viewRankPlayer.Open(playerOnRank);
        }

        highLightFrame.SetActive(true);
        highLightFrame.transform.parent = recordBtn.transform;
        highLightFrame.transform.localScale = UnityEngine.Vector3.one;
        highLightFrame.GetComponent<UnityEngine.RectTransform>().anchoredPosition = UnityEngine.Vector3.zero;
        highLightFrame.transform.SetAsFirstSibling();
    }

    public void OpenBtnClicked()
    {
        gameObject.SetActive(true);
        highLightFrame.SetActive(false);
        viewRankPlayer.city.eventsWindow.CloseBtnClcked();
        viewRankPlayer.city.ChooseBuilding(null);
    }

    public void CloseBtnClcked()
    {
        gameObject.SetActive(false);
        viewRankPlayer.gameObject.SetActive(false);
    }

    public override void OnTouchUpOutside(Finger f)
    {
        base.OnTouchUpOutside(f);
        //gameObject.SetActive(false);
    }
}
