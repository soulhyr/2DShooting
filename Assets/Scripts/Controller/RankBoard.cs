using TMPro;
using UnityEngine;

public class RankBoard : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text scoreText;
    public TMP_Text playerText;
    public TMP_Text stageText;

    public void SetText(int rank, int score, string player, int stage)
    {
        rankText.text = rank.ToString();
        scoreText.text = Utility.CommaString(score);
        playerText.text = player;
        stageText.text = stage.ToString();
    }
}