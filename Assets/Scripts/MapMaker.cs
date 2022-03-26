using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Jobs;
public class MapMaker : MonoBehaviour
{
    public static MapMaker Instance;
    [SerializeField] [Range(0, 20)] int mapSize = 10;
    [SerializeField] GameObject Floor;
    [SerializeField] GameObject LeftWall, RightWall, FrontWall, BackWall;
    [SerializeField] GameObject PlayerPrefab, EnemyPrefab, FinishPrefab, BoxPrefab, Finish;
    Vector3 FinishPos, PlayerPos, Enemy1Pos, Enemy2Pos;
    [SerializeField] [Range(0, 80)] int Obstacles;
    NavMeshSurface surface;
    NavMeshPath path;

    int horizontal, vertical;
    bool[,] isTilesBusy { get => new bool[mapSize, mapSize]; }
    enum TileType
    {
        Player,
        Enemy,
        Box
    }
    void Start()
    {
        Instance = this;
        path = new NavMeshPath();
        
        surface = GetComponent<NavMeshSurface>();
        Finish = Instantiate(FinishPrefab, new Vector3(((mapSize - 2) / 2f) * Random.Range(-1f, 1f), 0, (mapSize + 1) / 2f), Quaternion.identity);
        FinishPos = Finish.transform.position;
        MakeATile(TileType.Player);
        MakeATile(TileType.Enemy);
        MakeATile(TileType.Enemy);
        for (int i =0; i < Obstacles; i++) 
        {
             MakeATile(TileType.Box);
        }

        Floor.transform.localScale = Vector3.one * mapSize;
        LeftWall.transform.localScale = Vector3.one * mapSize;
        RightWall.transform.localScale = Vector3.one * mapSize;
        BackWall.transform.localScale = Vector3.one * mapSize;
        FrontWall.transform.localScale = Vector3.one * mapSize;
        LeftWall.transform.position = Vector3.left * mapSize * 0.5f;
        RightWall.transform.position = Vector3.right * mapSize * 0.5f;
        BackWall.transform.position = Vector3.back * mapSize * 0.5f;
        FrontWall.transform.position = Vector3.forward * mapSize * 0.5f;
    }
    public Vector3 GetPath(Vector3 MyPos)
    {
        surface.BuildNavMesh();
        Vector3 randPoint = Vector3.zero;
        bool isWorkingPoint = false;
        NavMeshPath path = new NavMeshPath();
        NavMeshPath path1 = new NavMeshPath();
        while (!isWorkingPoint)
        {
            GetFreeCell();
            randPoint = CellPosition();
            if (NavMesh.SamplePosition(randPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(MyPos, out NavMeshHit hit1, 0.1f, NavMesh.AllAreas))
                {
                    NavMesh.CalculatePath(hit1.position, hit.position, NavMesh.AllAreas, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        path1 = path;
                        isWorkingPoint = true;
                    }
                }
            }
        }
        return path.corners[path.corners.Length-1];
        //Пробовал прокидывать просто путь - не выходит. Просто InvalidPath. Сам вычисляет - всё классно
    }
    bool isPathAccess()
    {
        surface.BuildNavMesh();
        NavMeshHit hit, hit1;
        if (NavMesh.SamplePosition(FinishPos, out hit1, 0.1f, NavMesh.AllAreas))
        {
            if (NavMesh.SamplePosition(PlayerPos, out hit, 0.1f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(hit.position, hit1.position, NavMesh.AllAreas, path);
                return (path.status == NavMeshPathStatus.PathComplete);
            }
        }
        return false;
    }
    void MakeATile(TileType type)
    {
        switch (type)
        {
            case (TileType.Box):
                GameObject Box = Instantiate(BoxPrefab, Vector3.down * 10, Quaternion.identity);
                bool isFinishAccessible = false;
                int count = 0;
                while (!isFinishAccessible)
                {
                    GetFreeCell();
                    Box.transform.position = CellPosition();
                    if (isPathAccess())
                    {
                        isTilesBusy[horizontal, vertical] = true;
                        Box.gameObject.isStatic = true;
                        Box.transform.GetChild(0).gameObject.isStatic = true;
                        isFinishAccessible = true;
                    }
                    else
                    {
                        count++;
                    }
                    if (count == 10)
                    {
                        Box.transform.position += Vector3.up * 2;
                        isFinishAccessible = true;
                    }
                }
                break;
            case (TileType.Player):
                GetFreeCell();
                while (horizontal != 0) GetFreeCell();
                isTilesBusy[horizontal, vertical] = true;
                PlayerPos = Instantiate(PlayerPrefab, CellPosition(), Quaternion.identity).transform.position;
                break;
            case (TileType.Enemy):
                GetFreeCell();
                while (horizontal<mapSize*0.75f) GetFreeCell();
                isTilesBusy[horizontal, vertical] = true;
                Instantiate(EnemyPrefab, CellPosition(), Quaternion.identity);
                break;
        }
    }
    Vector3 CellPosition()
    {
        return new Vector3((-mapSize+1)/2f + vertical, 0, (-mapSize+1) / 2f + horizontal);
    }
    void GetFreeCell()
    {
        bool isDone = false;
        while (!isDone)
        {
            horizontal = Random.Range(0, mapSize);
            vertical = Random.Range  (0, mapSize);
            if (!isTilesBusy[horizontal,vertical])
            {
                isDone = true;
            }
        }
    }
    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }
}

