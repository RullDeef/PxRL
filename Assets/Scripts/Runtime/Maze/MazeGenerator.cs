using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    // Параметры генератора
    public GeneratorParams genParams;
    private Tilemap[] layers;
    public const int maxIterations = 10;

    // переменные для доступа к объектам на сцене
    private Grid grid;

    // сам мейз
    public Maze maze;

    public void Generate()
    {
        if (GatherRequiredComponents())
        {
            SetupTilemaps();

            Maze maze = new Maze();

            MazeRoom room = LoadRoomInfo("Start Room");
            PutRoom("Start Room", 0, 0);

            maze.rooms.Add(room);

            Debug.Log("Maze generated!");
        }
    }

    private bool GatherRequiredComponents()
    {
        if (genParams == null)
        {
            Debug.LogError("Нет параметров генерации уровня.");
            return false;
        }

        grid = GetComponentInChildren<Grid>();
        if (grid == null)
        {
            Debug.LogError("Нет компонента Grid в мейзе.");
            return false;
        }

        return true;
    }

    private void SetupTilemaps()
    {
        // добавить 3 слоя на карту
        InitLayers(3);
    }

    private MazeRoom LoadRoomInfo(string roomName)
    {
        string roomPath = $"Mazes/Rooms/{roomName}";
        GameObject roomPrefab = Resources.Load<GameObject>(roomPath);

        if (roomPrefab == null)
        {
            Debug.LogAssertion($"Комната '{roomName}' не найдена.");
            return null;
        }

        Grid roomGrid = roomPrefab.GetComponent<Grid>();
        if (roomGrid == null)
        {
            Debug.LogAssertion($"Комната '{roomName}' имеет неправильную структуру.");
            return null;
        }

        MazeRoom room = new MazeRoom();
        room.bounds = new BoundsInt(0, 0, 0, 0, 0, 0);

        foreach (Tilemap roomLayer in roomGrid.GetComponentsInChildren<Tilemap>())
        {
            BoundsInt bounds = roomLayer.cellBounds;
            room.bounds.xMin = Mathf.Min(room.bounds.xMin, bounds.xMin);
            room.bounds.xMax = Mathf.Max(room.bounds.xMax, bounds.xMax);
            room.bounds.yMin = Mathf.Min(room.bounds.yMin, bounds.yMin);
            room.bounds.yMax = Mathf.Max(room.bounds.yMax, bounds.yMax);
            room.bounds.zMin = Mathf.Min(room.bounds.zMin, bounds.zMin);
            room.bounds.zMax = Mathf.Max(room.bounds.zMax, bounds.zMax);
        }

        return room;
    }

    private void PutRoom(string roomName, int originX, int originY)
    {
        PutPatch($"Mazes/Rooms/{roomName}", originX, originY);
    }

    private void PutChunk(string chunkName, int originX, int originY)
    {
        PutPatch($"Maze Generator/Chunks/{chunkName}", originX, originY);
    }

    private void PutPatch(string patchPath, int originX, int originY)
    {
        GameObject chunkPrefab = Resources.Load<GameObject>(patchPath);
        if (chunkPrefab == null)
        {
            Debug.LogAssertion($"Патч по пути '{patchPath}' не найден.");
            return;
        }

        Grid chunkGrid = chunkPrefab.GetComponent<Grid>();
        if (chunkGrid == null)
        {
            Debug.LogAssertion($"Патч по пути '{patchPath}' имеет неправильную структуру.");
            return;
        }

        Tilemap[] chunkLayers = chunkGrid.GetComponentsInChildren<Tilemap>();
        for (int i = 0; i < chunkLayers.Length; i++)
        {
            // создать новый слой, если необходимо
            if (i == layers.Length)
            {
                Tilemap[] newLayers = new Tilemap[layers.Length + 1];
                layers.CopyTo(newLayers, 0);
                layers = newLayers;
            }

            // скопировать все клетки из данного слоя
            BoundsInt bounds = chunkLayers[i].cellBounds;
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int posSource = new Vector3Int(x, y, 0);
                    Vector3Int posTarget = new Vector3Int(x - originX, y - originY, 0);

                    Tile tile = chunkLayers[i].GetTile(posSource) as Tile;
                    layers[i].SetTile(posTarget, tile);
                }
            }
        }
    }

    // инициализирует пустые слои (для старта)
    private void InitLayers(int amount)
    {
        ClearLayers();
        layers = new Tilemap[amount];
        for (int i = 0; i < amount; i++)
            layers[i] = CreateLayer($"Layer {i}");
    }

    private Tilemap CreateLayer(string name = null)
    {
        GameObject map = new GameObject(name == null ? "Tilemap" : name);

        Tilemap tilemap = map.AddComponent<Tilemap>();
        map.AddComponent<TilemapRenderer>();

        map.transform.SetParent(grid.transform);
        return tilemap;
    }

    // очищает и удаляет все слои на сцене
    private void ClearLayers()
    {
        while (grid.transform.childCount > 0)
            #if UNITY_EDITOR
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
            #else
                Destroy(grid.transform.GetChild(0).gameObject);
            #endif
        layers = null;
    }

    // тестовая генерация пустот
    private bool[,] GenerateRooms()
    {
        bool[,] cells = new bool[genParams.dimensions.x, genParams.dimensions.y];

        for (int x = 0; x < genParams.dimensions.x; x++)
            for (int y = 0; y < genParams.dimensions.y; y++)
                cells[x, y] = false;
        
        for (int roomIndex = 0; roomIndex < genParams.roomsAmount; roomIndex++)
        {
            bool validSpace;
            int iteration = 0;
            int posX, posY;

            do
            {
                validSpace = true;

                posX = Random.Range(0, genParams.dimensions.x - genParams.roomSize.x);
                posY = Random.Range(0, genParams.dimensions.y - genParams.roomSize.y);

                for (int dx = 0; validSpace && dx < genParams.roomSize.x; dx++)
                    for (int dy = 0; validSpace && dy < genParams.roomSize.y; dy++)
                        if (cells[posX + dx, posY + dy])
                            validSpace = false;
            }
            while (!validSpace && iteration < maxIterations);

            if (!validSpace)
            {
                Debug.LogError("Не удалось создать уровень. Превышение числа итераций.");
                return null;
            }

            for (int dx = 0; dx < genParams.roomSize.x; dx++)
                for (int dy = 0; dy < genParams.roomSize.y; dy++)
                    cells[posX + dx, posY + dy] = true;
        }

        return cells;
    }
}
