using System.Collections.Generic;
using UnityEngine;

public interface ICheckPointBasedMovement
{
    List<Vector2> CheckPoints { get; }
    void AddCheckPoint(Vector2 checkpoint);
    void AddCheckPoint(IEnumerable<Vector2> checkpoints);
    void ClearCheckPoints();
}