using System;

[Serializable]
public class ScoreData
{
    public int score;
    public string name;
    public int stage;

    public ScoreData(int sc, string nm, int stg)
    {
        score = sc;
        name = nm;
        stage = stg;
    }
}