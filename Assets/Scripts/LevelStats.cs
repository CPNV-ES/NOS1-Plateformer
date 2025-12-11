using System;

[Serializable] // This allows Unity to turn it into JSON
public class LevelStats
{
    public string playerName;
    public float timeElapsed;
    public int kills;
    public int gems;
    public float percentage;
}