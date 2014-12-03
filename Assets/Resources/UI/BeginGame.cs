using UnityEngine;
using System.Collections;

public class BeginGame : MonoBehaviour 
{
    bool challengerBegan = false;
    UnityEngine.UI.Text textOnBtn;

    void Awake()
    {
        UnityEngine.UI.Button btn = GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(() => btnClicked());
        textOnBtn = GetComponentInChildren<UnityEngine.UI.Text>();        
    }

    public void btnClicked()
    {
        if (!challengerBegan)
        {
            challengerBegan = true;
            // 消息注册
            Globals.map.UnRegistDefenderEvent();
            Globals.map.RegistChallengerEvent();

            // 禁用所有的Defender UI
            UnityEngine.CanvasRenderer[] canvases = GameObject.FindObjectsOfType<UnityEngine.CanvasRenderer>();
            foreach (UnityEngine.CanvasRenderer canvas in canvases)
            {
                if(canvas.gameObject == textOnBtn.gameObject)
                {
                    textOnBtn.text = "结束游戏";
                }
                else if (canvas.gameObject != this.gameObject)
                {
                    canvas.gameObject.SetActive(false);
                }
            }

            // 
            // 魔术师出场
            GameObject magician_prefab = Resources.Load("Avatar/Mage_Girl") as GameObject;
            GameObject magician = GameObject.Instantiate(magician_prefab) as GameObject;

            Cell magician_birth = Globals.map.entryOfMaze;
            magician.name = "Mage_Girl";
            magician.transform.position = magician_birth.GetFloorPos();

            // Defender的相机删除
            Globals.cameraForDefender.gameObject.SetActive(false);

            // 相机跟随
            GameObject camera_follow_prefab = Resources.Load("CameraFollowMagician") as GameObject;
            GameObject camera_follow = GameObject.Instantiate(camera_follow_prefab) as GameObject;
            Globals.cameraFollowMagician.beginFollow(magician.transform);        
        }
        else
        {
            Globals.asyncLoad.ToLoadSceneAsync("Game");
        }
    }
}
