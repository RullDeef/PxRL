using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct GeneratorTiles
{
    public Tile wall;
}

[CreateAssetMenu(fileName = "Generator Params", menuName = "Maze/Generator params")]
[Serializable]
public class GeneratorParams : ScriptableObject
{
    public Vector2Int dimensions;

    public int roomsAmount;
    public Vector2Int roomSize;

    public GeneratorTiles tiles;
}
