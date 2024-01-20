using System.Collections.Generic;
using UnityEngine;

public interface ICheckPointBasedMovement
{
    void AddCheckPoint(Vector2 checkpoint);
    void AddCheckPoint(IEnumerable<Vector2> checkpoints);
    void ClearCheckPoints();
}