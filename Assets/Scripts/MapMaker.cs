using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapMaker : MonoBehaviour
{
    public static MapMaker Instance;
    MapConfig ActualConfig;
    GameObject[] CreatedWalls;
    Player playerOnScene;
    Enemy[] Enemies;
    [SerializeField] Transform Floor;
    Transform LeftWall, RightWall, FrontWall, BackWall;
    [SerializeField] GameObject PlayerPrefab, EnemyPrefab, FinishPrefab, BoxPrefab;
    GameObject FinishOnScene;
    NavMeshSurface surface;
    NavMeshPath path;

    int RealCountOfWalls;
    int horizontal, vertical;
    bool[,] BusyPlaces;

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
        CreateScene();
    }

    public static void BuildNewScene(MapConfig NewConfig)
    {
        Instance.ActualConfig = NewConfig;
        Instance.CreateScene();
    }

    public static void BuildNewScene()
    {
        Instance.CreateScene();
    }

    void CreateScene()
    {
        CheckConfig();
        CreateWalls();
        CreateAndPlacePlayer();
        CreateAndPlaceEnemies();
        RebuildMapWallsAndFloor();
        PlaceWalls();
    }

    void CheckConfig()
    {
        if (ActualConfig == null)
        {
            ActualConfig = new MapConfig();
        }
        RealCountOfWalls = (ActualConfig.MapScale * ActualConfig.MapScale)/ 100 * ActualConfig.ObstaclesPercent;
        BusyPlaces = new bool[ActualConfig.MapScale, ActualConfig.MapScale];
    }

    void CreateWalls()
    {
        if (CreatedWalls == null)
        {
            CreatedWalls = new GameObject[RealCountOfWalls];
        }
        if (CreatedWalls.Length != RealCountOfWalls)
        {
            System.Array.Resize(ref CreatedWalls, RealCountOfWalls);
        }
        for (int i=0; i< CreatedWalls.Length;i++)
        {
            if (CreatedWalls[i] == null)
            {
                CreatedWalls[i] = Instantiate(BoxPrefab);
            }
            CreatedWalls[i].transform.position = Vector3.down * 10;
            CreatedWalls[i].gameObject.isStatic = false;
        }
    }

    void CreateAndPlacePlayer()
    {
        if (playerOnScene == null)
        {
            playerOnScene = (Instantiate(PlayerPrefab) as GameObject).GetComponent<Player>();
        }
        GenerateFreeCell();
        while (vertical != 0) GenerateFreeCell();
        BusyPlaces[horizontal, vertical] = true;
        playerOnScene.ResetState();
        playerOnScene.transform.position = ActualCellPosition();
    }

    void CreateAndPlaceEnemies()
    {
        if (Enemies == null)
        {
            Enemies = new Enemy[ActualConfig.EnemiesCount];
        }
        if (Enemies.Length > ActualConfig.EnemiesCount)
        {
            for (int i=Enemies.Length-1; i>ActualConfig.EnemiesCount-1;i--)
            {
                Destroy(Enemies[i]);
            }
        }
        if (Enemies.Length > ActualConfig.EnemiesCount)
        {
            System.Array.Resize(ref Enemies, ActualConfig.EnemiesCount);
        }
        for (int i=0 ;i< Enemies.Length; i++)
        {
            if (Enemies[i] == null)
            {
                Enemies[i] = (Instantiate(EnemyPrefab)as GameObject).GetComponent<Enemy>();
            }
            else 
            {
                Enemies[i].ResetState();
            }
            GenerateFreeCell();
            while (vertical < ActualConfig.MapScale*0.75f) GenerateFreeCell();
            BusyPlaces[horizontal, vertical] = true;
            Enemies[i].transform.position = ActualCellPosition();
        }
    }

    void RebuildMapWallsAndFloor()
    {
        if (FinishOnScene == null)
        {
            FinishOnScene = Instantiate(FinishPrefab);
            FinishOnScene.transform.SetParent(transform);
        }
        FinishOnScene.transform.position = new Vector3(((ActualConfig.MapScale - 2) / 2f) * Random.Range(-1f, 1f), 0, (ActualConfig.MapScale + 1) / 2f);
        FinishOnScene.isStatic = true;
        Floor.localScale = Vector3.one * ActualConfig.MapScale;
        if (LeftWall == null) LeftWall = CreateCollider();
        LeftWall.localScale = Vector3.one * ActualConfig.MapScale;
        LeftWall.position = Vector3.left * ActualConfig.MapScale * 0.5f;
        LeftWall.LookAt(Vector3.left * ActualConfig.MapScale);
        if (RightWall == null) RightWall = CreateCollider();
        RightWall.localScale = Vector3.one * ActualConfig.MapScale;
        RightWall.position = Vector3.right * ActualConfig.MapScale * 0.5f;
        RightWall.LookAt(Vector3.right * ActualConfig.MapScale);
        if (BackWall == null) BackWall = CreateCollider();
        BackWall.localScale = Vector3.one * ActualConfig.MapScale;
        BackWall.position = Vector3.back * ActualConfig.MapScale * 0.5f;
        BackWall.LookAt(Vector3.back * ActualConfig.MapScale);
        if (FrontWall == null) FrontWall = CreateCollider();
        FrontWall.localScale = Vector3.one * ActualConfig.MapScale;
        FrontWall.position = Vector3.forward * ActualConfig.MapScale * 0.5f;
        FrontWall.LookAt(Vector3.forward * ActualConfig.MapScale);
    }

    Transform CreateCollider()
    {
        GameObject Collider = new GameObject();
        Collider.transform.parent = transform;
        Collider.AddComponent<MeshCollider>().sharedMesh = UnityEngine.Resources.GetBuiltinResource<Mesh>("Quad.fbx");
        return Collider.transform;
    }

    void PlaceWalls()
    {
        for (int i=0; i< CreatedWalls.Length;i++)
        {
            while (true)
            {
                GenerateFreeCell();
                CreatedWalls[i].transform.position = ActualCellPosition();
                if (!MapIsValid())
                {
                    continue;
                }
                BusyPlaces[horizontal, vertical] = true;
                break;
            }
        }
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
            GenerateFreeCell();
            randPoint = ActualCellPosition();
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

    bool MapIsValid()
    {
        surface.BuildNavMesh();
        if (!IsPathAccessible(playerOnScene.transform.position, FinishOnScene.transform.position))
        {
            return false;
        }
        for (int i=0; i<Enemies.Length; i++)
        {
            if (!IsPathAccessible(Enemies[i].transform.position, playerOnScene.transform.position))
            {
                return false;
            }
        }
        return true;
    }

    bool IsPathAccessible(Vector3 StartPath, Vector3 EndPath)
    {
        if (NavMesh.SamplePosition(StartPath, out NavMeshHit hit1, 0.1f, NavMesh.AllAreas))
        {
            if (NavMesh.SamplePosition(EndPath, out NavMeshHit hit2, 0.1f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(hit2.position, hit1.position, NavMesh.AllAreas, path);
                return (path.status == NavMeshPathStatus.PathComplete);
            }
        }
        return false;
    }

    Vector3 ActualCellPosition()
    {
        return new Vector3((-ActualConfig.MapScale+1)/2f + horizontal, 0, (-ActualConfig.MapScale+1) / 2f + vertical);
    }

    void GenerateFreeCell()
    {
        while (true)
        {
            horizontal = Random.Range(0, ActualConfig.MapScale);
            vertical = Random.Range  (0, ActualConfig.MapScale);
            if (!BusyPlaces[horizontal,vertical])
            {
                return;
            }
        }
    }
}