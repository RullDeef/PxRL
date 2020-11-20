using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    // Параметры генератора
    public GeneratorParams genParams;
    public const int maxIterations = 10;

    // переменные для доступа к объектам на сцене
    private Grid grid;

    public void Generate()
    {
        if (GatherRequiredComponents())
        {
            SetupTilemaps();
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
        while (grid.transform.childCount > 0)
            #if UNITY_EDITOR
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
            #else
                Destroy(grid.transform.GetChild(0).gameObject);
            #endif

        // добавить 3 слоя на карту
        Tilemap background = CreateLayer("Layer 0");
        bool[,] rooms = GenerateRooms();

        for (int x = 0; x < rooms.GetLength(0); x++)
            for (int y = 0; y < rooms.GetLength(1); y++)
                if (rooms[x, y] == false)
                    background.SetTile(new Vector3Int(x, y, 0), genParams.tiles.wall);
    }

    private Tilemap CreateLayer(string name = null)
    {
        GameObject map = new GameObject(name == null ? "Tilemap" : name);

        Tilemap tilemap = map.AddComponent<Tilemap>();
        map.AddComponent<TilemapRenderer>();

        map.transform.SetParent(grid.transform);
        return tilemap;
    }

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
