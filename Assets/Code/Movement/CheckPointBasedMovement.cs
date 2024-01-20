using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckPointBasedMovement : MonoBehaviour, ICheckPointBasedMovement
{
    [SerializeField] private float speed;
    private readonly List<Vector2> checkPoints = new();

    void Update()
    {
        if (checkPoints.Any())
        {
            if ((Vector2)transform.position != checkPoints[0])
                transform.position = Vector2.MoveTowards(transform.position, checkPoints[0], speed * Time.deltaTime);
            else
                checkPoints.Remove(checkPoints[0]);
        }
    }

    public void AddCheckPoint(IEnumerable<Vector2> checkpoints) => checkPoints.AddRange(checkpoints);

    public void AddCheckPoint(Vector2 checkpoint) => checkPoints.Add(checkpoint);

    public void ClearCheckPoints() => checkPoints.Clear();
}
