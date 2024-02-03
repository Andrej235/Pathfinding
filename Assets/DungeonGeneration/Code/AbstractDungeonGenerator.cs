using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tileMapVisualizer;
    [SerializeField] protected Vector2Int startPosition;

    public void GenerateDungeon()
    {
        tileMapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
