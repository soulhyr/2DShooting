using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankMain : MonoBehaviour
{
    public TMP_Text title;
    public Transform parentTransform;
    public GameObject rankBoardPrefab;
    public Button btnTouch;
    
    public float speed = 1f; // 색상 변화 속도

    private Color colorA = new Color(0xCC / 255f, 0x9A / 255f, 0x00 / 255f);
    private Color colorB = new Color(0xCC / 255f, 0x12 / 255f, 0x00 / 255f);
    private List<ScoreData> scoreDataList;
    
    void Awake()
    {
        scoreDataList = DataManager.Instance.LoadScoreData();
        scoreDataList.Sort((a, b) =>
        {
            int cmp = b.score.CompareTo(a.score);
            if (cmp != 0) return cmp;

            cmp = b.stage.CompareTo(a.stage);
            if (cmp != 0) return cmp;

            return a.name.CompareTo(b.name);
        });
        btnTouch.onClick.AddListener(TouchTextClicked);
    }
    
    void Start()
    {
        for (int i = 0; i < scoreDataList.Count; i++)
        {
            ScoreData scoreData = scoreDataList[i];
            GameObject go = Instantiate(rankBoardPrefab, parentTransform);
            RankBoard rb = go.GetComponent<RankBoard>();
            rb.SetText(i + 1, scoreData.score, scoreData.name, scoreData.stage);
        }
    }
    
    void Update() => title.color = Color.Lerp(colorA, colorB, Mathf.PingPong(Time.time * speed, 1f));

    public void TouchTextClicked() => Utility.ChangeScene(GameDef.Scenes.Title);
}