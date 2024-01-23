using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private ICheckPointBasedMovement movement;

    [SerializeField]
    private float visibilityRange;
    private List<PathNode> path;

    void Start()
    {
        if (!gameObject.TryGetComponent(out movement))
            Debug.LogError($"{nameof(EnemyController)} requires a component of type {nameof(ICheckPointBasedMovement)} to function");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, visibilityRange);

        if (path is null || path.Count < 2)
            return;

        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(
                GridMaster.Instance.Grid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (GridMaster.Instance.Grid.CellSize / 2),
                GridMaster.Instance.Grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (GridMaster.Instance.Grid.CellSize / 2));
    }

    private void FixedUpdate()
    {
        Transform target = null;
        var overlaps = Physics2D.OverlapCircleAll(transform.position, visibilityRange);
        if (!overlaps.Any())
            return;

        foreach (var x in overlaps)
        {
            if (x.TryGetComponent(out IHostileTarget y))
            {
                target = x.transform;
                break;
            }
        }

        //If enemy has a target, pathfind to it otherwise pathfind to position of its previous final checkpoint
        if (target != null)
        {
            path = (List<PathNode>)GridMaster.Instance.Pathfinding.FindPath((Vector2)transform.position, target.position);

            //If enemy couldnt find a path to its target, find a path to its previous final checkpoint
            if (path is null || path.Count < 2)
                FindPathToPreviousFinalCheckpoint();
        }
        else
            FindPathToPreviousFinalCheckpoint();

        void FindPathToPreviousFinalCheckpoint()
        {
            if (!movement.CheckPoints.Any())
                return;

            path = (List<PathNode>)GridMaster.Instance.Pathfinding.FindPath((Vector2)transform.position, movement.CheckPoints.Last());
        }

        //If enemy still doesnt have a path return
        movement.ClearCheckPoints();
        if (path is null || path.Count < 2)
            return;

        path.Remove(path[0]); //Skip the first node so the enemy doesn't just circle around for some reason

        //Add new checkpoints to movement script
        var checkpoints = path.Select(node => GridMaster.Instance.Grid.GetWorldPosition(node.x, node.y));
        movement.AddCheckPoint(checkpoints);
    }
}
