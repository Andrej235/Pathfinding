using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckPointBasedMovement : MonoBehaviour, ICheckPointBasedMovement
{
    [SerializeField] private float speed;
    public List<Vector2> CheckPoints { get; } = new();

    void Update()
    {
        if (CheckPoints.Any())
        {
            if ((Vector2)transform.position != CheckPoints[0])
                transform.position = Vector2.MoveTowards(transform.position, CheckPoints[0], speed * Time.deltaTime);
            else
                CheckPoints.Remove(CheckPoints[0]);
        }
    }

    public void AddCheckPoint(IEnumerable<Vector2> checkpoints) => CheckPoints.AddRange(checkpoints);

    public void AddCheckPoint(Vector2 checkpoint) => CheckPoints.Add(checkpoint);

    public void ClearCheckPoints() => CheckPoints.Clear();
}
