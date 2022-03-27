[System.Serializable]
public class MapConfig
{
    public int MapScale;
    public int ObstaclesPercent;
    public int EnemiesCount;

    public MapConfig (int scale, int obstacles, int enemies)
    {
        MapScale = scale;
        ObstaclesPercent = obstacles;
        EnemiesCount = enemies;
    }

    public MapConfig()
    {
        MapScale = 10;
        ObstaclesPercent = 20;
        EnemiesCount = 2;
    }
}