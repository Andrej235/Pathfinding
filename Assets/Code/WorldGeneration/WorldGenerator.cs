using Assets.Code.Utility;
using Assets.Code.WorldGeneration;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
public class WorldGenerator : MonoBehaviour
{
    [SerializeField] int islands;
    [SerializeField] int distanceBetweenIslands;
    [SerializeField] private RandomWalkParametersSO parameters;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase basicTile;
    private WorldTilemapVisualizer? tilemapVisualizer;

    public void Generate()
    {
        tilemapVisualizer ??= new(tilemap);
        tilemapVisualizer.Clear();

        List<Vector2Int> islandCenters = new() { Vector2Int.zero };
        Vector2Int currentCenter = islandCenters.GetRandomElement();

        for (int i = 0; i < islands; i++)
        {
            var island = RunRandomWalk(parameters, currentCenter);
            tilemapVisualizer.PaintTiles(island, basicTile);

            currentCenter = islandCenters.GetRandomElement() + Directions.RandomDirection * distanceBetweenIslands;
            islandCenters.Add(currentCenter);
        }
    }

    private HashSet<Vector2Int> RunRandomWalk(RandomWalkParametersSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);

            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    private WorldGenerator generator;
    private void Awake() => generator = (WorldGenerator)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
            generator.Generate();
    }
}
