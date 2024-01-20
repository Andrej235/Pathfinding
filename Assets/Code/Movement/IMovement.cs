using UnityEngine;

public interface IMovement
{
    void MoveTo(Vector2 targetPos);
    void MoveBy(float x, float y);
    void MoveBy(Vector2 moveBy);
}