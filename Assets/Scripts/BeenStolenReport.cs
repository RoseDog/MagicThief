public class BeenStolenReport : CustomEventTrigger
{
    public UnityEngine.GameObject eventPrefab;
    public UnityEngine.UI.GridLayoutGroup layout;
    
    public void Open()
    {
        transform.parent.parent.gameObject.SetActive(true);
        Globals.city.mainCanvas.gameObject.SetActive(false);


        System.Collections.ArrayList dateArray = new System.Collections.ArrayList();
        foreach (System.Collections.DictionaryEntry entry in Globals.self.beenStolenReports)
        {
            dateArray.Add(System.Convert.ToDateTime(entry.Key));
        }

        dateArray.Sort();

        for (int idx = dateArray.Count-1; idx >= 0; --idx)
        {
            ReplayData replay = Globals.self.beenStolenReports[dateArray[idx].ToString()] as ReplayData;
            AddEvent(replay);
        }        
    }

    public void AddEvent(ReplayData replay)
    {
        UnityEngine.GameObject report = Instantiate(eventPrefab) as UnityEngine.GameObject;        
        report.transform.SetParent(layout.transform);
        report.transform.localScale = UnityEngine.Vector3.one;

        UnityEngine.UI.Text time_stamp = Globals.getChildGameObject<UnityEngine.UI.Text>(report, "time_stamp");
        UnityEngine.UI.Text stolen_by_others_event = Globals.getChildGameObject<UnityEngine.UI.Text>(report, "stolen_by_others_event");
        UnityEngine.UI.Text stealing_cash = Globals.getChildGameObject<UnityEngine.UI.Text>(report, "stealing_cash");


        System.DateTime then = System.Convert.ToDateTime(replay.date);
        System.TimeSpan date_diff = System.DateTime.Now - then;

        if (date_diff.Days != 0)
        {
            Globals.languageTable.SetText(time_stamp, "few_days_ago",
            new System.String[] { date_diff.Days.ToString() });
        }
        else if (date_diff.Hours != 0)
        {
            Globals.languageTable.SetText(time_stamp, "few_hours_ago",
            new System.String[] { date_diff.Hours.ToString() });
        }
        else
        {
            Globals.languageTable.SetText(time_stamp, "few_minutes_ago");
        }

        if (replay.StealingCashInSafebox > 0)
        {
            stolen_by_others_event.color = UnityEngine.Color.red;
            Globals.languageTable.SetText(stolen_by_others_event, "stolen_by_others_event_guards_failed", new System.String[] { replay.thief.name, replay.thief.roseCount.ToString() });
        }
        else
        {
            stolen_by_others_event.color = UnityEngine.Color.green;
            Globals.languageTable.SetText(stolen_by_others_event, "stolen_by_others_event_guards_success", new System.String[] { replay.thief.name, replay.thief.roseCount.ToString() });
        }          
        
        Globals.languageTable.SetText(stealing_cash, "stealing_cash",
            new System.String[] { (replay.StealingCashInSafebox + replay.PickedCash).ToString("F0") });
    }

    public void CloseBtnClicked()
    {
        transform.parent.parent.gameObject.SetActive(false);
        Globals.city.mainCanvas.gameObject.SetActive(true);
    }
}
